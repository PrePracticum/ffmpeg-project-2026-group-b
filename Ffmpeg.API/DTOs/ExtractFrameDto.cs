using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class ExtractFrameDto
    {
        public IFormFile InputFile { get; set; }
        public string TimeStamp { get; set; }
    }
}