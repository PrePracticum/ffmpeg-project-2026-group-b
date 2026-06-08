namespace FFmpeg.API.DTOs
{
    public class ChangeResolutionDto
    {
        public IFormFile VideoFile { get; set; }
        public int Width { get; set; } = 1280; // ברירת מחדל כמו בדוגמה
        public int Height { get; set; } = 720;  // ברירת מחדל כמו בדוגמה
    }
}