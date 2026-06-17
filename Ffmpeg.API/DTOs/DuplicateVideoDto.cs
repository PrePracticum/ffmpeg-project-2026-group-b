using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class DuplicateVideoDto
    {
        public IFormFile VideoFile { get; set; }
        public int Duplicates { get; set; } = 2; // ברירת מחדל לפעמיים (פיצול מסך ל-2)
    }
}
