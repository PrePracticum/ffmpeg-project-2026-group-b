namespace FFmpeg.Core.Models
{
    public class LimitBitrateModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string Bitrate { get; set; } = "1M"; // ערך ברירת מחדל
    }
}