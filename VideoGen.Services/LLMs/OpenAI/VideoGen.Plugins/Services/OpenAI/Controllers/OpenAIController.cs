using VideoGen.Plugins.Services.OpenAI.Entities;
using VideoGen.Plugins.Services.OpenAI.Interfaces;
using VideoGen.Auth.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VideoGen.Plugins.Services.OpenAI.Controllers
{
    [Route("/api/services/openai")]
    [Authorize(Policy = nameof(AuthConstants.Policy.ManageUsers))]
    public class OpenAIController : Controller
    {
        readonly IOpenAIPlatformRepository _repo;

        public OpenAIController(IOpenAIPlatformRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("get-platforms")]
        public async Task<IActionResult> GetPlatforms()
        {
            try
            {
                var platforms = await _repo.GetAll();
                return Json(new { success = true, data = platforms });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("save-platform")]
        public async Task<IActionResult> SavePlatform([FromBody] OpenAIPlatform platform)
        {
            try
            {
                if (platform.Id > 0)
                {
                    await _repo.Update(platform);
                }
                else
                {
                    platform.Id = await _repo.Add(platform);
                }
                return Json(new { success = true, data = platform });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("delete-platform")]
        public async Task<IActionResult> DeletePlatform([FromBody] DeletePlatformRequest request)
        {
            try
            {
                await _repo.Delete(request.Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class DeletePlatformRequest
    {
        public int Id { get; set; }
    }
}
