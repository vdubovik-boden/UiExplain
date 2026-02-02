using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using UiExplain.Api.Models;

namespace UiExplain.Api.Services;

public class UiAnalysisService : IUiAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _settings;
    private readonly ILogger<UiAnalysisService> _logger;

    public UiAnalysisService(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<UiAnalysisService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<ExplainResult> AnalyzeUiAsync(string caption)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "system", content = "You are an expert in UX design and web accessibility. Analyze UI screenshots and provide detailed insights." },
                    new { role = "user", content = $"Analyze this UI description: {caption}. Return ONLY a JSON object with the following structure: {{ \"Summary\": \"string\", \"MainUIElements\": [\"string\"], \"UXIssues\": [\"string\"], \"AccessibilitySuggestions\": [\"string\"] }}. Do not include any other text." }
                },
                model = "llama-3.1-8b-instant",
                temperature = 0.2
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"gsk_{_settings.GroqApiKey}");

            _logger.LogInformation("Sending UI analysis request to Groq with caption: {Caption}", caption);
            var response = await _httpClient.SendAsync(request);
            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Groq API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                throw new Exception($"Groq API error: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var resultJson = groqResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "{}";

            var cleanedJson = CleanJson(resultJson);
            ExplainResult? result;
            try
            {
                result = JsonSerializer.Deserialize<ExplainResult>(cleanedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse Groq JSON response. Raw content: {Content}", resultJson);
                result = new ExplainResult { Summary = "AI response could not be parsed." };
            }

            result ??= new ExplainResult();
            _logger.LogInformation("UI analysis completed in {ElapsedMs}ms with summary: {Summary}", stopwatch.ElapsedMilliseconds, result.Summary);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing UI with caption: {Caption}", caption);
            throw;
        }
    }

    private static string CleanJson(string content)
    {
        var trimmed = content.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            return trimmed;
        }

        var firstLineEnd = trimmed.IndexOf('\n');
        var lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
        if (firstLineEnd >= 0 && lastFence > firstLineEnd)
        {
            return trimmed.Substring(firstLineEnd + 1, lastFence - firstLineEnd - 1).Trim();
        }

        return trimmed.Trim('`');
    }
}