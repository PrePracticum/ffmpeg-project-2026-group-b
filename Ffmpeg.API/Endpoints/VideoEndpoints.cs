using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FFmpeg.API.DTOs;
using FFmpeg.Core.Interfaces;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using FFmpeg.Infrastructure.Services;

namespace FFmpeg.API.Endpoints
{
    public static class VideoEndpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapPost("/api/video/watermark", AddWatermark)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB

            app.MapPost("/api/video/reverse", ReverseVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB
            app.MapPost("/api/video/animatedText", AddAnimatedText)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB
            app.MapPost("/api/video/green-screen", GreenScreen)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB    

            app.MapPost("/api/video/merge", MergeVideos)
        .DisableAntiforgery()
        .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB
        }

        private static async Task<IResult> MergeVideos(
            HttpContext context,
            [FromForm] MergeVideosDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                // 1. וולידציה לקלטים
                if (dto.FirstVideo == null || dto.SecondVideo == null)
                {
                    return Results.BadRequest("Both videos are required");
                }

                // 2. שמירת קבצי הקלט באמצעות השמות האמיתיים מה-Service שלך
                string firstVideoName = await fileService.SaveUploadedFileAsync(dto.FirstVideo);
                string secondVideoName = await fileService.SaveUploadedFileAsync(dto.SecondVideo);

                // 3. יצירת שם ייחודי לקובץ הפלט
                string extension = Helper.GetOutputExtension(dto.Format);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                // רשימה למחיקה אוטומטית של הקבצים הזמניים בסיום
                List<string> filesToCleanup = new List<string> { firstVideoName, secondVideoName, outputFileName };

                try
                {
                    // 4. יצירת ה-Command דרך ה-Factory (ניצור את המתודה הזו בשלב הבא)
                    var command = ffmpegService.CreateMergeVideosCommand();

                    var result = await command.ExecuteAsync(new MergeVideosModel
                    {
                        FirstVideoPath = firstVideoName,
                        SecondVideoPath = secondVideoName,
                        OutputVideoPath = outputFileName,
                        Mode = dto.Mode ?? "horizontal"
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg merge command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to merge videos: " + result.ErrorMessage, statusCode: 500);
                    }

                    // 5. קריאת קובץ הפלט הממוזג
                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    // 6. ניקוי קבצים זמניים
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    // 7. החזרת הקובץ למשתמש
                    string contentType = Helper.GetContentType(dto.Format);
                    return Results.File(fileBytes, contentType, "merged_" + dto.FirstVideo.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing merge video request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MergeVideos endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
    }

    private static async Task<IResult> AddWatermark(
        HttpContext context,
        [FromForm] WatermarkDto dto)
    {
        var fileService = context.RequestServices.GetRequiredService<IFileService>();
        var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>(); // or a specific logger type

        try
        {
            // Validate request
            if (dto.VideoFile == null || dto.WatermarkFile == null)
            {
                return Results.BadRequest("Video file and watermark file are required");
            }

            // Save uploaded files
            string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
            string watermarkFileName = await fileService.SaveUploadedFileAsync(dto.WatermarkFile);

            // Generate output filename
            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

            // Track files to clean up
            List<string> filesToCleanup = new List<string> { videoFileName, watermarkFileName, outputFileName };

            try
            {
                // Create and execute the watermark command
                var command = ffmpegService.CreateWatermarkCommand();
                var result = await command.ExecuteAsync(new WatermarkModel
                {
                    InputFile = videoFileName,
                    WatermarkFile = watermarkFileName,
                    OutputFile = outputFileName,
                    XPosition = dto.XPosition,
                    YPosition = dto.YPosition,
                    IsVideo = true,
                    VideoCodec = "libx264"
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to add watermark: " + result.ErrorMessage, statusCode: 500);
                }

                // Read the output file
                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                // Clean up temporary files
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                // Return the file
                return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing watermark request");
                // Clean up on error
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in AddWatermark endpoint");
            return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
        }

    }

    private static async Task<IResult> ReverseVideo(
        HttpContext context,
        [FromForm] ReverseVideoDto dto)
    {
        var fileService = context.RequestServices.GetRequiredService<IFileService>();
        var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        try
        {
            if (dto.VideoFile == null)
            {
                return Results.BadRequest("Video file is required");
            }

            string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

            List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

            try
            {
                var command = ffmpegService.CreateReverseVideoCommand();
                var result = await command.ExecuteAsync(new ReverseVideoModel
                {
                    InputFile = videoFileName,
                    OutputFile = outputFileName
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg reverse command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to reverse video: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "video/mp4", "reversed_" + dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing reverse video request");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ReverseVideo endpoint");
            return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
        }
    }
    private static async Task<IResult> AddAnimatedText(
        HttpContext context,
        [FromForm] AnimatedTextDto dto)
    {
        var fileService = context.RequestServices.GetRequiredService<IFileService>();
        var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        try
        {
            if (dto.VideoFile == null)
            {
                return Results.BadRequest("Video file is required");
            }
            if (string.IsNullOrWhiteSpace(dto.Text))
            {
                return Results.BadRequest("Text is required");
            }

            string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);


            List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

            try
            {
                var command = ffmpegService.CreateAnimatedTextCommand();
                var result = await command.ExecuteAsync(new AnimatedTextModel
                {
                    InputFile = videoFileName,

                    Text = dto.Text,
                    Color = dto.Color,
                    FontSize = dto.FontSize,
                    XPosition = dto.XPosition,
                    YPosition = dto.YPosition,
                    IsAnimated = dto.IsAnimated,
                    AnimationType = dto.AnimationType,
                    AnimationSpeed = dto.AnimationSpeed,
                    OutputFile = outputFileName
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg animated text command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to add animated text: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "video/mp4", "animated_" + dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing animated text request");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in AddAnimatedText endpoint");
            return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
        }
    }


    private static async Task<IResult> GreenScreen(
        HttpContext context,
        [FromForm] GreenScreenDto dto)
    {
        var fileService = context.RequestServices.GetRequiredService<IFileService>();
        var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        try
        {
            if (dto.InputFile == null || dto.BackgroundFile == null)
            {
                return Results.BadRequest("Input video and background video are required");
            }

            string inputFileName = await fileService.SaveUploadedFileAsync(dto.InputFile);
            string backgroundFileName = await fileService.SaveUploadedFileAsync(dto.BackgroundFile);

            string extension = Path.GetExtension(dto.InputFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

            List<string> filesToCleanup = new List<string> { inputFileName, backgroundFileName, outputFileName };

            try
            {
                var command = ffmpegService.CreateGreenScreenCommand();
                var result = await command.ExecuteAsync(new GreenScreenModel
                {
                    InputFile = inputFileName,
                    BackgroundFile = backgroundFileName,
                    OutputFile = outputFileName
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg green-screen command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to process green screen: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "video/mp4", "greenscreen_" + dto.InputFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing green screen request");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GreenScreen endpoint");
            return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
        }
    }
}
}