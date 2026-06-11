namespace FFmpeg.API.DTOs;

public class RotationRequestDto
{
    public string InputVideoName { get; set; } = string.Empty;
    public int RotationAngle { get; set; }
    public string OutputVideoName { get; set; } = string.Empty;
}