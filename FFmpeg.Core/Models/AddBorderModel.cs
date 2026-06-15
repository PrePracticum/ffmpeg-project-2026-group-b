using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class AddBorderModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int BorderSize { get; set; } = 20;
        public string BorderColor { get; set; } = "black";
    }
}
