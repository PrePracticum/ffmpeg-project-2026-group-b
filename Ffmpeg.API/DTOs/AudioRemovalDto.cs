using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class AudioRemovalDto
    {
        public IFormFile VideoFile { get; set; }
    }
}