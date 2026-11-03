using VideoGen.Plugins.Models;

namespace VideoGen.Plugins
{
    public interface ITranscriptGeneration
    {
        Transcript Generate(string prompt, string options, Action<Progress> progress);
    }
}
