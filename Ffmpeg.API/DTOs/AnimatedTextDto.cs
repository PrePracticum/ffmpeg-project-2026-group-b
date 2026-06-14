namespace FFmpeg.API.DTOs
{
    public class AnimatedTextDto
    {
        public IFormFile VideoFile { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public int FontSize { get; set; }
        public int XPosition { get; set; } = 540;
        public int YPosition { get; set; } = 960;
        
        // Animation properties
        public bool IsAnimated { get; set; } = false;
        public FFmpeg.Core.Models.AnimationType AnimationType { get; set; } = FFmpeg.Core.Models.AnimationType.MoveLeft;
        public int AnimationSpeed { get; set; } = 100; // pixels per second
    }
}