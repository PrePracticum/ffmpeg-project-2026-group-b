using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class ChangeAudioFormatDto
    {
        public IFormFile AudioFile { get; set; }
        public string TargetFormat { get; set; } // לדוגמה: ".wav"
    }
}