using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public enum AnimationType
    {
        None = 0,
        MoveLeft = 1,
        MoveRight = 2,
        SlideDown = 3,
        SlideUp = 4
    }
    public class AnimatedTextModel
    {
        public string InputFile { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public int FontSize { get; set; }
        public int XPosition { get; set; } = 540;
        public int YPosition { get; set; } = 960;
        public string OutputFile { get; set; }
        
        // Animation properties
        public bool IsAnimated { get; set; } = false;
        public AnimationType AnimationType { get; set; } = AnimationType.MoveLeft;
        public int AnimationSpeed { get; set; } = 100; // pixels per second

    }
}
