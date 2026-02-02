using UiExplain.Api.Models;

namespace UiExplain.Api.Services;

public class UiExplainOrchestrator : IUiExplainOrchestrator
{
    private readonly IImageCaptionService _imageCaptionService;
    private readonly IUiAnalysisService _uiAnalysisService;
    private readonly ILogger<UiExplainOrchestrator> _logger;

    public UiExplainOrchestrator(IImageCaptionService imageCaptionService, IUiAnalysisService uiAnalysisService, ILogger<UiExplainOrchestrator> logger)
    {
        _imageCaptionService = imageCaptionService;
        _uiAnalysisService = uiAnalysisService;
        _logger = logger;
    }

    public async Task<ExplainResult> ExplainUiAsync(Stream imageStream)
    {
        _logger.LogInformation("Starting UI explanation process");

        var caption = await _imageCaptionService.CaptionImageAsync(imageStream);
        _logger.LogInformation("Generated caption: {Caption}", caption);

        var result = await _uiAnalysisService.AnalyzeUiAsync(caption);
        _logger.LogInformation("Completed UI analysis with {MainElementsCount} main elements, {IssuesCount} issues, {SuggestionsCount} suggestions",
            result.MainUIElements.Count, result.UXIssues.Count, result.AccessibilitySuggestions.Count);

        return result;
    }
}