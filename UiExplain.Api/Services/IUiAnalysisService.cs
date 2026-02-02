using UiExplain.Api.Models;

namespace UiExplain.Api.Services;

public interface IUiAnalysisService
{
    Task<ExplainResult> AnalyzeUiAsync(string caption);
}