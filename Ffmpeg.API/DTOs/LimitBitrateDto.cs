using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class LimitBitrateDto
    {
        public IFormFile VideoFile { get; set; }
        public string Bitrate { get; set; } = "1M";
    }
}