namespace FFmpeg.Core.Models
{
    public class ChangeResolutionModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}