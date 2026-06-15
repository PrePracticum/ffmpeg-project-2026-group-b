using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class AddBorderDto
    {
        public IFormFile VideoFile { get; set; }
        public int BorderSize { get; set; } = 20;
        public string BorderColor { get; set; } = "black";
    }
}