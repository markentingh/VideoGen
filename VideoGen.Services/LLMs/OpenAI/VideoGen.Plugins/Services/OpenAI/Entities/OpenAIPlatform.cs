namespace VideoGen.Plugins.Services.OpenAI.Entities
{
    public class OpenAIPlatform
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Endpoint { get; set; }
        public string? Model { get; set; }
        public string? ModelName { get; set; }
        public string ApiKey { get; set; } = "";
    }
}
