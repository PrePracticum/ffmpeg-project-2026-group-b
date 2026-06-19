using FFmpeg.API.DTOs;
using FFmpeg.Core.Interfaces;

namespace FFmpeg.API.Endpoints
{
    public static class AudioEffectEndpoints
    {
        public static void MapAudioEffectEndpoints(this IEndpointRouteBuilder app)
        {
            // מגדירים נתיב מסוג POST שהכתובת שלו תהיה /api/audio/echo
            app.MapPost("/api/audio/echo", async (AudioEffectRequest request, IAudioEffectService audioService) =>
            {
                try
                {
                    // אנחנו קוראים לפונקציה שיצרנו קודם בתשתית ומעבירים לה את הנתונים
                    await audioService.AddEchoEffectAsync(request.InputVideoName, request.Duration, request.OutputVideoName);
                    
                    // אם הכל עבד מעולה, נחזיר הודעת הצלחה
                    return Results.Ok(new { Message = "Echo effect applied successfully!" });
                }
                catch (Exception ex)
                {
                    // אם הייתה שגיאה ב-FFmpeg נחזיר הודעת שגיאה
                    return Results.BadRequest(new { Error = ex.Message });
                }
            });
        }
    }
}