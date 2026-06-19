using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class TimestampModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int FontSize { get; set; } = 24;
        public string FontColor { get; set; } = "white";
        public int XPosition { get; set; } = 10;
        public int YPosition { get; set; } = 10;
    }
}
