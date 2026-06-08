using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class ChangeSpeedModel
    {
        public string InputFile { get; set; }
        public double SpeedMultiplier { get; set; }
        public string OutputFile { get; set; }
    }
}
