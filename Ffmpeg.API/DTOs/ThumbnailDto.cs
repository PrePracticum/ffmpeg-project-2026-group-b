using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class ThumbnailDto
    {
        public IFormFile VideoFile { get; set; }
        public string TimePosition { get; set; } = "00:00:05";
    }
}