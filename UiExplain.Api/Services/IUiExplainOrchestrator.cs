using UiExplain.Api.Models;

namespace UiExplain.Api.Services;

public interface IUiExplainOrchestrator
{
    Task<ExplainResult> ExplainUiAsync(Stream imageStream);
}