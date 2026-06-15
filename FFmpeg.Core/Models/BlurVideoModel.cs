namespace FFmpeg.Core.Models
{
    public class BlurVideoModel
    {
        public string InputFile { get; set; }
        public int Sigma { get; set; }
        public string OutputFile { get; set; }
    }
}