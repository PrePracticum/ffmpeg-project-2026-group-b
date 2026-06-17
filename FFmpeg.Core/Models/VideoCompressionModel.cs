using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class VideoCompressionModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int Crf { get; set; } = 28;
    }
}
