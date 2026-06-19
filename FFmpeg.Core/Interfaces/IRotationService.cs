namespace FFmpeg.Core.Interfaces;

public interface IRotationService
{
    bool RotateVideo(string inputName, int angle, string outputName);
}