namespace VideoGen.Data.Entities.Services
{
    public class Service
    {
        public int Id { get; set; }
        public string Key { get; set; } = "";
        public string Version { get; set; } = "";
        public string Name { get; set; } = "";
        public int Type { get; set; }
        public string GitRepo { get; set; } = "";
        public string Icon { get; set; } = "";
        public bool HasSchema { get; set; }
        public string InstalledFiles { get; set; } = "[]";
        public DateTime Installed { get; set; }
    }
}
