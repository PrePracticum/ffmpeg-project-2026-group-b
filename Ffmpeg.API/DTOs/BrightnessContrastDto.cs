using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class BrightnessContrastDto
    {
        public IFormFile VideoFile { get; set; }
        public double Brightness { get; set; } = 0;  // range: -1.0 to 1.0
        public double Contrast { get; set; } = 1;    // range: -1000.0 to 1000.0
    }
}
