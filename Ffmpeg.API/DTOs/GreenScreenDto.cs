using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class GreenScreenDto
    {
        public IFormFile InputFile { get; set; }
        public IFormFile BackgroundFile { get; set; }
    }
}