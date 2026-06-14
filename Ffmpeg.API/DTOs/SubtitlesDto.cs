using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class SubtitlesDto
    {
        public IFormFile VideoFile { get; set; }

        public IFormFile SubtitlesFile { get; set; }
    }
}