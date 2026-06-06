using FFmpeg.API.DTOs;
using FFmpeg.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FFmpeg.API.Endpoints;

public static class RotationEndpoints
{
    public static void MapRotationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/video/rotate", (RotationRequestDto request, IRotationService rotationService) =>
        {
            bool success = rotationService.RotateVideo(request.InputVideoName, request.RotationAngle, request.OutputVideoName);

            if (success)
            {
                return Results.Ok(new { Message = "הוידאו סובב בהצלחה!" });
            }

            return Results.StatusCode(500); // שגיאת שרת אם הפעולה נכשלה
        });
    }
}