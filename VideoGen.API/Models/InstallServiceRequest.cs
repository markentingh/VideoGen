namespace VideoGen.API.Models
{
    public class InstallServiceRequest
    {
        public string Key { get; set; } = "";
        public string Version { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string GitRepo { get; set; } = "";
    }
}
