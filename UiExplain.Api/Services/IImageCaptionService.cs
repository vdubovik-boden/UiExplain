namespace UiExplain.Api.Services;

public interface IImageCaptionService
{
    Task<string> CaptionImageAsync(Stream imageStream);
}