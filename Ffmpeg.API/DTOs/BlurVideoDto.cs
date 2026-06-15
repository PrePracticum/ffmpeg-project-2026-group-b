using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class BlurVideoDto
    {
        public IFormFile VideoFile { get; set; }
        public int Sigma { get; set; }
    }
}