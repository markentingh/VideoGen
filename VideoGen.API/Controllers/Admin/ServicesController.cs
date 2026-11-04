using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using VideoGen.API.Models;
using VideoGen.Auth.Policies;
using VideoGen.Data.Entities.Services;
using VideoGen.Data.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace VideoGen.API.Controllers.Admin
{
    [Route("/api/admin/services")]
    [Authorize(Policy = nameof(AuthConstants.Policy.ManageUsers))]
    public class ServicesController : ApiController
    {
        private static List<AvailableService>? _servicesCache;
        private static readonly object _cacheLock = new();

        readonly IConfiguration _config;
        readonly IWebHostEnvironment _env;
        readonly IHttpClientFactory _httpClientFactory;
        readonly IServicesRepository _servicesRepo;

        public ServicesController(
            IConfiguration config,
            IWebHostEnvironment env,
            IHttpClientFactory httpClientFactory,
            IServicesRepository servicesRepo
        )
        {
            _config = config;
            _env = env;
            _httpClientFactory = httpClientFactory;
            _servicesRepo = servicesRepo;
        }

        [HttpGet("get-available-services")]
        public async Task<IActionResult> GetAvailableServices()
        {
            try
            {
                lock (_cacheLock)
                {
                    if (_servicesCache != null)
                    {
                        return Json(new ApiResponse { success = true, data = _servicesCache });
                    }
                }

                var client = _httpClientFactory.CreateClient();
                var tasks = GetServiceUrls().Select(url => FetchServicesAsync(client, url)).ToList();
                var results = await Task.WhenAll(tasks);
                var services = results.SelectMany(list => list).ToList();

                lock (_cacheLock)
                {
                    _servicesCache = services;
                }

                return Json(new ApiResponse { success = true, data = services });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-urls")]
        public IActionResult GetUrls()
        {
            try
            {
                return Json(new ApiResponse { success = true, data = new { urls = _config["Services:Urls"] ?? "" } });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { success = false, message = ex.Message });
            }
        }

        [HttpPost("save-urls")]
        public async Task<IActionResult> SaveUrls([FromBody] UpdateServiceUrlsRequest request)
        {
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, $"appsettings.{_env.EnvironmentName}.json");
                if (!System.IO.File.Exists(filePath))
                {
                    filePath = Path.Combine(_env.ContentRootPath, "appsettings.json");
                }

                var node = JsonNode.Parse(await System.IO.File.ReadAllTextAsync(filePath)) ?? new JsonObject();
                if (node["Services"] is not JsonObject services)
                {
                    services = new JsonObject();
                    node["Services"] = services;
                }
                services["Urls"] = request.Urls ?? "";
                await System.IO.File.WriteAllTextAsync(filePath, node.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

                lock (_cacheLock)
                {
                    _servicesCache = null;
                }

                return Json(new ApiResponse { success = true, message = "Settings saved" });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-installed")]
        public async Task<IActionResult> GetInstalled()
        {
            try
            {
                var services = await _servicesRepo.GetAll();
                return Json(new ApiResponse { success = true, data = services });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { success = false, message = ex.Message });
            }
        }

        [HttpGet("icon")]
        public IActionResult Icon([FromQuery] string file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(file))
                {
                    return NotFound();
                }
                var repoRoot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..")) + Path.DirectorySeparatorChar;
                var fullPath = Path.GetFullPath(Path.Combine(repoRoot, file));
                if (!fullPath.StartsWith(repoRoot) || !System.IO.File.Exists(fullPath))
                {
                    return NotFound();
                }
                if (!new FileExtensionContentTypeProvider().TryGetContentType(fullPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                return PhysicalFile(fullPath, contentType!);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost("install")]
        public async Task<IActionResult> Install([FromBody] InstallServiceRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Key))
                {
                    return Json(new ApiResponse { success = false, message = "Service key is required" });
                }

                if (!_env.IsDevelopment())
                {
                    return Json(new ApiResponse { success = false, message = "Services can only be installed in a development environment" });
                }

                var repoRoot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, ".."));

                // 1. download the service repository into the executable folder
                var downloadDir = Path.Combine(AppContext.BaseDirectory, "Downloads", $"{request.Key}-{request.Version}");
                if (Directory.Exists(downloadDir)) Directory.Delete(downloadDir, true);
                Directory.CreateDirectory(downloadDir);

                var localPath = ResolveLocalRepoPath(request.GitRepo);
                if (localPath != null)
                {
                    CopyDirectory(localPath, downloadDir);
                }
                else
                {
                    var psi = new ProcessStartInfo("git")
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    psi.ArgumentList.Add("clone");
                    psi.ArgumentList.Add("--depth");
                    psi.ArgumentList.Add("1");
                    psi.ArgumentList.Add(request.GitRepo);
                    psi.ArgumentList.Add(downloadDir);
                    var process = Process.Start(psi);
                    if (process == null)
                    {
                        return Json(new ApiResponse { success = false, message = "Failed to start git" });
                    }
                    var stderr = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();
                    if (process.ExitCode != 0)
                    {
                        return Json(new ApiResponse { success = false, message = $"Failed to clone repository: {stderr.Trim()}" });
                    }
                }

                var installedFiles = new List<string>();

                // 2. copy React components into the VideoGen.Web.Client project
                var repoClientDir = Path.Combine(downloadDir, "VideoGen.Web.Client");
                if (Directory.Exists(repoClientDir))
                {
                    CopyDirectory(repoClientDir, Path.Combine(repoRoot, "VideoGen.Web.Client"), installedFiles, repoRoot);
                }

                // 3. copy .cs files into the VideoGen.Plugins project (mirrors project root;
                //    service files must live under Services/{ServiceName}/ in the repo)
                var repoPluginsDir = Path.Combine(downloadDir, "VideoGen.Plugins");
                if (Directory.Exists(repoPluginsDir))
                {
                    CopyDirectory(repoPluginsDir, Path.Combine(repoRoot, "VideoGen.Plugins"), installedFiles, repoRoot);
                }

                // 4. copy SQL scripts into VideoGen.SQL and run the deploy scripts
                var hasSchema = false;
                var repoSqlDir = Path.Combine(downloadDir, "VideoGen.SQL");
                if (Directory.Exists(repoSqlDir))
                {
                    hasSchema = true;
                    var sqlDir = Path.Combine(repoRoot, "VideoGen.SQL");
                    CopyDirectory(repoSqlDir, sqlDir, installedFiles, repoRoot);
                    var (exitCode, output) = await RunProcess("cmd.exe", "/c deploy.bat", sqlDir);
                    if (exitCode != 0)
                    {
                        return Json(new ApiResponse { success = false, message = $"Database migration failed: {output}" });
                    }
                }

                var icon = "";
                if (!string.IsNullOrWhiteSpace(request.IconUrl))
                {
                    try
                    {
                        var ext = Path.GetExtension(new Uri(request.IconUrl).AbsolutePath);
                        if (string.IsNullOrEmpty(ext)) ext = ".png";
                        icon = $"{request.Key}{ext}";
                        var bytes = await _httpClientFactory.CreateClient().GetByteArrayAsync(request.IconUrl);
                        foreach (var dir in GetIconDirs())
                        {
                            Directory.CreateDirectory(dir);
                            await System.IO.File.WriteAllBytesAsync(Path.Combine(dir, icon), bytes);
                        }
                    }
                    catch
                    {
                        icon = "";
                    }
                }

                await _servicesRepo.Upsert(new Service
                {
                    Key = request.Key,
                    Version = request.Version,
                    Name = request.Name,
                    Type = GetServiceTypeId(request.Type),
                    GitRepo = request.GitRepo,
                    Icon = icon,
                    HasSchema = hasSchema,
                    InstalledFiles = JsonSerializer.Serialize(installedFiles)
                });

                return Json(new ApiResponse { success = true, message = $"{request.Name} installed" });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { success = false, message = ex.Message });
            }
        }

        private string? ResolveLocalRepoPath(string gitrepo)
        {
            if (Path.IsPathRooted(gitrepo))
            {
                return Directory.Exists(gitrepo) ? gitrepo : null;
            }
            foreach (var candidate in new[]
            {
                Path.Combine(_env.ContentRootPath, gitrepo),
                Path.Combine(_env.ContentRootPath, "..", gitrepo)
            })
            {
                var full = Path.GetFullPath(candidate);
                if (Directory.Exists(full)) return full;
            }
            return null;
        }

        private List<string> GetIconDirs()
        {
            var dirs = new List<string> { Path.Combine(_env.ContentRootPath, "wwwroot", "services") };
            var clientPublic = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "VideoGen.Web.Client", "public", "services"));
            if (Directory.Exists(Path.GetDirectoryName(clientPublic)!))
            {
                dirs.Add(clientPublic);
            }
            return dirs;
        }

        private static void CopyDirectory(string source, string target, List<string>? installedFiles = null, string? relativeTo = null)
        {
            Directory.CreateDirectory(target);
            foreach (var file in Directory.GetFiles(source))
            {
                var dest = Path.Combine(target, Path.GetFileName(file));
                System.IO.File.Copy(file, dest, true);
                installedFiles?.Add(relativeTo != null ? Path.GetRelativePath(relativeTo, dest) : dest);
            }
            foreach (var dir in Directory.GetDirectories(source))
            {
                CopyDirectory(dir, Path.Combine(target, Path.GetFileName(dir)), installedFiles, relativeTo);
            }
        }

        private static async Task<(int exitCode, string output)> RunProcess(string fileName, string arguments, string workingDirectory)
        {
            var psi = new ProcessStartInfo(fileName, arguments)
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            var process = Process.Start(psi);
            if (process == null)
            {
                return (-1, $"Failed to start {fileName}");
            }
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            var stderr = await stderrTask;
            return (process.ExitCode, string.IsNullOrWhiteSpace(stderr) ? (await stdoutTask).Trim() : stderr.Trim());
        }

        private static int GetServiceTypeId(string type) => type switch
        {
            "llms" => 1,
            "transcript-generation" => 2,
            "image-library" => 3,
            "image-generation" => 4,
            "video-library" => 5,
            "video-generation" => 6,
            "video-transitions" => 7,
            "music-library" => 8,
            "music-generation" => 9,
            "sound-fx-library" => 10,
            "sound-fx-generation" => 11,
            "text-to-speech" => 12,
            "text-captions" => 13,
            _ => 0
        };

        private static async Task<List<AvailableService>> FetchServicesAsync(HttpClient client, string url)
        {
            var services = new List<AvailableService>();
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var json = await client.GetStringAsync(url, cts.Token);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != JsonValueKind.Array) return services;
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var name = GetString(item, "name");
                    var type = GetString(item, "type");
                    var gitrepo = GetString(item, "gitrepo");
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(gitrepo))
                    {
                        continue;
                    }
                    services.Add(new AvailableService
                    {
                        key = GetString(item, "key"),
                        version = GetString(item, "version"),
                        name = name,
                        type = type,
                        summary = GetString(item, "summary"),
                        gitrepo = gitrepo,
                        iconurl = GetString(item, "iconurl")
                    });
                }
            }
            catch
            {
                // skip unavailable or invalid service URLs
            }
            return services;
        }

        private string[] GetServiceUrls()
        {
            return (_config["Services:Urls"] ?? "")
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        private static string GetString(JsonElement element, string property)
        {
            return element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString() ?? ""
                : "";
        }
    }
}
