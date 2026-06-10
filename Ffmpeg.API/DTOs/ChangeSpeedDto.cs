namespace FFmpeg.API.DTOs
{
    public class ChangeSpeedDto
    {
        public IFormFile VideoFile { get; set; }
        public double SpeedMultiplier { get; set; }
    }
}
