using System.Net.Http.Headers;
using System.Text.Json;

namespace UiExplain.Api.Models;

public class HuggingFaceResponse
{
    public string GeneratedText { get; set; } = string.Empty;
}