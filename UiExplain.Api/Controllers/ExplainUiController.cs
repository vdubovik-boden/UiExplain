using Microsoft.AspNetCore.Mvc;
using UiExplain.Api.Services;

namespace UiExplain.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExplainUiController : ControllerBase
{
    private readonly IUiExplainOrchestrator _uiExplainOrchestrator;

    public ExplainUiController(IUiExplainOrchestrator uiExplainOrchestrator)
    {
        _uiExplainOrchestrator = uiExplainOrchestrator;
    }

    [HttpPost]
    public async Task<IActionResult> ExplainUi(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("No image file provided.");
        }

        if (image.Length > 10 * 1024 * 1024) // 10MB limit
        {
            return BadRequest("Image file too large. Maximum size is 10MB.");
        }

        //if (!image.ContentType.StartsWith("image/"))
        //{
        //    return BadRequest("Invalid file type. Only image files are allowed.");
        //}

        using var stream = image.OpenReadStream();
        var result = await _uiExplainOrchestrator.ExplainUiAsync(stream);

        return Ok(result);
    }
}