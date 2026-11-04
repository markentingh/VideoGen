using System.Reflection;

namespace VideoGen.Plugins
{
    public static class LLM
    {
        public static string Prompt(string systemPrompt, string assistantPrompt, string userPrompt, string serviceKey, string options = "")
        {
            var implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypes)
                .Where(type => typeof(ILLM).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false });

            foreach (var type in implementations)
            {
                var key = type.GetProperty("Key", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;
                if (key != serviceKey) continue;

                var method = type.GetMethod("Prompt", BindingFlags.Public | BindingFlags.Static)
                    ?? throw new InvalidOperationException($"ILLM implementation \"{type.Name}\" does not define a static Prompt method");
                return (string)method.Invoke(null, new object[] { systemPrompt, assistantPrompt, userPrompt, options })!;
            }

            throw new InvalidOperationException($"No ILLM implementation found for service key \"{serviceKey}\"");
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(type => type != null)!;
            }
        }
    }
}
