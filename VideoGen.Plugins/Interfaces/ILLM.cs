namespace VideoGen.Plugins
{
    public interface ILLM
    {
        static abstract string Key { get; }
        static abstract string Prompt(string systemPrompt, string assistantPrompt, string userPrompt, string options);
    }
}
