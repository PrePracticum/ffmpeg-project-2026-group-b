using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class ConvertFormatDto
    {
        public IFormFile VideoFile { get; set; }
        // למשל: "avi", "mkv", "mov"
        public string OutputFormat { get; set; } 
    }
}