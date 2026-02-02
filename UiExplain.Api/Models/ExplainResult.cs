namespace UiExplain.Api.Models;

public class ExplainResult
{
    public string Summary { get; set; } = string.Empty;
    public List<string> MainUIElements { get; set; } = new();
    public List<string> UXIssues { get; set; } = new();
    public List<string> AccessibilitySuggestions { get; set; } = new();
}