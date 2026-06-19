using System.Diagnostics;
using FFmpeg.Core.Interfaces;

namespace FFmpeg.Infrastructure.Services;

public class RotationService : IRotationService
{
    public bool RotateVideo(string inputName, int angle, string outputName)
    {
        // הרכבת הפקודה של FFmpeg עם הנתונים שהתקבלו
        string arguments = $"-i \"{inputName}\" -vf \"rotate={angle}*PI/180\" \"{outputName}\"";

        var processInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();
        
        // המתנה לסיום הפעולה
        process.WaitForExit();

        // החזרת 'אמת' אם הפעולה הסתיימה בהצלחה (ExitCode 0)
        return process.ExitCode == 0;
    }
}