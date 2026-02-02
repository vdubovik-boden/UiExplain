using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using UiExplain.Api.Models;

namespace UiExplain.Api.Services;

public class ImageCaptionService : IImageCaptionService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _settings;
    private readonly ILogger<ImageCaptionService> _logger;

    public ImageCaptionService(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<ImageCaptionService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<string> CaptionImageAsync(Stream imageStream)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            var bytes = ms.ToArray();
            var base64 = Convert.ToBase64String(bytes);

            var requestBody = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = "Describe this image in one sentence." },
                            new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64}" } }
                        }
                    }
                },
                model = "moonshotai/Kimi-K2.5:fireworks-ai",
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://router.huggingface.co/v1/chat/completions")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"hf_{_settings.HuggingFaceApiKey}");

            _logger.LogInformation("Sending image caption request to Hugging Face");
            var response = await _httpClient.SendAsync(request);
            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Hugging Face API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                throw new Exception($"Hugging Face API error: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var caption = groqResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "No caption generated";

            _logger.LogInformation("Image caption generated in {ElapsedMs}ms: {Caption}", stopwatch.ElapsedMilliseconds, caption);
            return caption;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image caption");
            throw;
        }
    }
}