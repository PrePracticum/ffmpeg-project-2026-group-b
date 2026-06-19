using Microsoft.AspNetCore.Http;

public class ReplaceAudioDto
{
    public IFormFile VideoFile { get; set; }
    public IFormFile AudioFile { get; set; }
}
