using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class VideoCompressionDto
    {
        public IFormFile VideoFile { get; set; }
        public int Crf { get; set; } = 28;
    }
}