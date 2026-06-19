using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class TimestampDto
    {
        // Video input file
        public IFormFile VideoFile { get; set; }

        // Optional output file name (with or without extension). If not provided, a unique name will be generated.
        public string OutputFileName { get; set; }

        public int FontSize { get; set; } = 24;
        public string FontColor { get; set; } = "white";
        public int XPosition { get; set; } = 10;
        public int YPosition { get; set; } = 10;
    }
}
