using System;
namespace FFmpeg.Core.Models
{
    public class MergeVideosModel
    {
        public string FirstVideoPath { get; set; }
        public string SecondVideoPath { get; set; }
        public string OutputVideoPath { get; set; }
        public string Mode { get; set; } // "horizontal" או "vertical"
    }
}