using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class CutVideoModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
