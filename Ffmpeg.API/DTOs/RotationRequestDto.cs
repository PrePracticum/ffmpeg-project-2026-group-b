using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs;

public class RotationRequestDto
{
    public IFormFile? VideoFile { get; set; }
    public int RotationAngle { get; set; }
}