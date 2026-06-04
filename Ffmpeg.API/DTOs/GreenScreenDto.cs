using Microsoft.AspNetCore.Http;

namespace Ffmpeg.API.DTOs
{
    public class GreenScreenDto
    {
        public IFormFile InputFile { get; set; }
        public IFormFile BackgroundFile { get; set; }
    }
}