using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class CropVideoModel
    {
        public string InputFile { get; set; } = string.Empty;

        public string OutputFile { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}