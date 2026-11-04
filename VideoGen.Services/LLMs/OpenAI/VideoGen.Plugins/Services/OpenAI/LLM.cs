using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace VideoGen.Plugins.Services.OpenAI
{
    public class LLM : ILLM
    {
        public static string Key => "openai";

        public static string Prompt(string systemPrompt, string assistantPrompt, string userPrompt, string options)
        {
            var opts = string.IsNullOrWhiteSpace(options)
                ? new JsonObject()
                : JsonNode.Parse(options) as JsonObject ?? new JsonObject();

            var endpoint = opts["endpoint"]?.GetValue<string>() ?? "https://api.openai.com/v1";
            var model = opts["model"]?.GetValue<string>() ?? "";
            var apiKey = opts["apiKey"]?.GetValue<string>() ?? "";

            using var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }

            var messages = new List<object>();
            if (!string.IsNullOrWhiteSpace(systemPrompt)) messages.Add(new { role = "system", content = systemPrompt });
            if (!string.IsNullOrWhiteSpace(assistantPrompt)) messages.Add(new { role = "assistant", content = assistantPrompt });
            messages.Add(new { role = "user", content = userPrompt });

            var body = JsonSerializer.Serialize(new { model, messages });
            var response = client.PostAsync(
                $"{endpoint.TrimEnd('/')}/chat/completions",
                new StringContent(body, Encoding.UTF8, "application/json")
            ).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            response.EnsureSuccessStatusCode();

            var doc = JsonNode.Parse(json);
            return doc?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? "";
        }
    }
}
