using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ChatFPT.Service.Interfaces;

public class GptService : IGPTInterface
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GptService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("API key not found in configuration.");
    }

    public async Task<string> GetGptResponse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Error: Message cannot be empty.";
        }

        var requestBody = new
        {
            model = "gpt-3.5-turbo", // Change based on your OpenAI model
            messages = new[]
            {
                new { role = "user", content = message }
            },
            temperature = 0.7
        };

        var requestJson = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            return $"Error: GPT API request failed with status code {response.StatusCode}";
        }

        var responseString = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseString))
        {
            return "Error: Received empty response from GPT API.";
        }

        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            var choices = root.GetProperty("choices");

            if (choices.GetArrayLength() > 0)
            {
                return choices[0].GetProperty("message").GetProperty("content").GetString() ?? "Error: GPT response content is null.";
            }
        }
        catch (Exception ex)
        {
            return $"Error: Failed to parse GPT API response. {ex.Message}";
        }

        return "Error: GPT API response did not contain expected data.";
    }
}
