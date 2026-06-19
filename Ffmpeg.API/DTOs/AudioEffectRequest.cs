namespace FFmpeg.API.DTOs
{
    public class AudioEffectRequest
    {
        public string InputVideoName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string OutputVideoName { get; set; } = string.Empty;
    }
}