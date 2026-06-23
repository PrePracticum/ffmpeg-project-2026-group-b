using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class MixAudioDto
    {
        public IFormFile AudioFile1 { get; set; }
        public IFormFile AudioFile2 { get; set; }
    }
}