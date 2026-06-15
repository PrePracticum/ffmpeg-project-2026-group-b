using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class ChangeVolumeDto
    {
        public IFormFile VideoFile { get; set; }
        public double VolumeMultiplier { get; set; }
    }
}
