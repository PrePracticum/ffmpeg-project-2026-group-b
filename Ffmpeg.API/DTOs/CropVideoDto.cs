using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class CropVideoDto
    {
        public IFormFile VideoFile { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}