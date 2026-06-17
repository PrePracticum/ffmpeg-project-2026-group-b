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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FFmpeg.API.Endpoints
{
    public static class VideoEndpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapPost("/api/video/watermark", AddWatermark)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/reverse", ReverseVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB

            app.MapPost("/api/audio/change-format", ChangeAudioFormat)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/crop", CropVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/cut", CutVideo)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/animatedText", AddAnimatedText)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/green-screen", GreenScreen)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/extract-frame", ExtractFrame)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/subtitles", AddSubtitles)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB

            app.MapPost("/api/video/change-resolution", ChangeResolution)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB   
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB    

            app.MapPost("/api/video/change-speed", ChangeSpeed)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/change-volume", ChangeVolume)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/thumbnail", CreateThumbnail)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));
            app.MapPost("/api/video/remove-audio", RemoveAudio)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/convert-format", ConvertFormat)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB    
            app.MapPost("/api/video/extract-audio", ExtractAudio)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/brightness-contrast", AdjustBrightnessContrast)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/add-border", AddBorder)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/blur", BlurVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/compress", CompressVideo)
    .DisableAntiforgery()
    .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/duplicate", DuplicateVideo)
    .DisableAntiforgery()
    .WithMetadata(new RequestSizeLimitAttribute(104857600));
        }

        private static async Task<IResult> ChangeAudioFormat(HttpContext context, [FromForm] ChangeAudioFormatDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.AudioFile == null) return Results.BadRequest("Audio file is required");

                string audioFileName = await fileService.SaveUploadedFileAsync(dto.AudioFile);
                string extension = string.IsNullOrEmpty(dto.TargetFormat) ? ".wav" : dto.TargetFormat;
                if (!extension.StartsWith(".")) extension = "." + extension;
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { audioFileName, outputFileName };

                try
                {
                    string fullOutputPath = fileService.GetFullOutputPath(outputFileName);

                    string baseDir = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(fullOutputPath));
                    string fullInputPath = audioFileName;
                    if (!string.IsNullOrEmpty(baseDir))
                    {
                        string[] foundFiles = System.IO.Directory.GetFiles(baseDir, audioFileName, System.IO.SearchOption.AllDirectories);
                        if (foundFiles.Length > 0)
                        {
                            fullInputPath = foundFiles[0];
                        }
                    }

                    var command = ffmpegService.CreateChangeAudioFormatCommand();
                    var result = await command.ExecuteAsync(new ChangeAudioFormatModel
                    {
                        InputFile = fullInputPath,
                        OutputFile = fullOutputPath
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg command failed: {ErrorMessage}", result.ErrorMessage);
                        return Results.Problem("Failed to change format: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    string finalName = "converted_audio" + extension;
                    return Results.File(fileBytes, "audio/" + extension.Trim('.'), finalName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing audio format request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ChangeAudioFormat endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> AddWatermark(
            HttpContext context,
            [FromForm] WatermarkDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null || dto.WatermarkFile == null)
                {
                    return Results.BadRequest("Video file and watermark file are required");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string watermarkFileName = await fileService.SaveUploadedFileAsync(dto.WatermarkFile);

                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, watermarkFileName, outputFileName };

                try
                {
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

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing watermark request");
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

        private static async Task<IResult> AddSubtitles(
            HttpContext context,
            [FromForm] SubtitlesDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null || dto.SubtitlesFile == null)
                {
                    return Results.BadRequest("Video file and subtitles file are required");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string subtitlesFileName = await fileService.SaveUploadedFileAsync(dto.SubtitlesFile);

                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, subtitlesFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateSubtitlesCommand();
                    var result = await command.ExecuteAsync(new SubtitlesModel
                    {
                        InputFile = videoFileName,
                        SubtitlesFile = subtitlesFileName,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg subtitles command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to add subtitles: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", "subtitled_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing subtitles request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddSubtitles endpoint");
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

        private static async Task<IResult> CropVideo(
            HttpContext context,
            [FromForm] CropVideoDto dto)
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

                logger.LogInformation("Original FileName: {FileName}", dto.VideoFile.FileName);
                logger.LogInformation("ContentType: {ContentType}", dto.VideoFile.ContentType);
                logger.LogInformation("Length: {Length}", dto.VideoFile.Length);
                string videoFileName =
                    await fileService.SaveUploadedFileAsync(dto.VideoFile);

                string extension =
                    Path.GetExtension(dto.VideoFile.FileName);

                string outputFileName =
                    await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup =
                    new() { videoFileName, outputFileName };

                try
                {
                    var command =
                        ffmpegService.CreateCropVideoCommand();

                    var result =
                        await command.ExecuteAsync(
                            new CropVideoModel
                            {
                                InputFile = videoFileName,
                                OutputFile = outputFileName,
                                Width = dto.Width,
                                Height = dto.Height,
                                X = dto.X,
                                Y = dto.Y
                            });

                    if (!result.IsSuccess)
                    {
                        logger.LogError(
                            "FFmpeg crop command failed: {ErrorMessage}",
                            result.ErrorMessage);

                        return Results.Problem(
                            "Failed to crop video: " +
                            result.ErrorMessage,
                            statusCode: 500);
                    }

                    byte[] fileBytes =
                        await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(
                        fileBytes,
                        "video/mp4",
                        "cropped_" + dto.VideoFile.FileName);
                }
                catch
                {
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CropVideo endpoint");

                return Results.Problem(
                    "An error occurred: " + ex.Message,
                    statusCode: 500);
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

        private static async Task<IResult> ExtractFrame(
            HttpContext context,
            [FromForm] ExtractFrameDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.InputFile == null)
                {
                    return Results.BadRequest("Video file is required");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.InputFile);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(".png");
                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateExtractFrameCommand();
                    var result = await command.ExecuteAsync(new ExtractFrameModel
                    {
                        InputFile = videoFileName,
                        TimeStamp = dto.TimeStamp,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg extract frame failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to extract frame: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    return Results.File(fileBytes, "image/png", outputFileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing extract frame request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ExtractFrame endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> ChangeResolution(
            HttpContext context,
            [FromForm] ChangeResolutionDto dto)
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

                if (dto.Width <= 0 || dto.Height <= 0)
                {
                    return Results.BadRequest("Width and Height must be greater than 0");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateChangeResolutionCommand();
                    var result = await command.ExecuteAsync(new ChangeResolutionModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        Width = dto.Width,
                        Height = dto.Height
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg change resolution failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to change resolution: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", "resized_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing change resolution request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ChangeResolution endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> ChangeSpeed(
            HttpContext context,
            [FromForm] ChangeSpeedDto dto)
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

                if (dto.SpeedMultiplier <= 0)
                {
                    return Results.BadRequest("Speed multiplier must be greater than zero");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateChangeSpeedCommand();
                    var result = await command.ExecuteAsync(new ChangeSpeedModel
                    {
                        InputFile = videoFileName,
                        SpeedMultiplier = dto.SpeedMultiplier,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg change speed command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to change video speed: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", "speed_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing change speed request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ChangeSpeed endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> ChangeVolume(
            HttpContext context,
            [FromForm] ChangeVolumeDto dto)
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

                if (dto.VolumeMultiplier <= 0)
                {
                    return Results.BadRequest("Volume multiplier must be greater than zero");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateChangeVolumeCommand();
                    var result = await command.ExecuteAsync(new ChangeVolumeModel
                    {
                        InputFile = videoFileName,
                        VolumeMultiplier = dto.VolumeMultiplier,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg change volume command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to change video volume: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", "volume_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing change volume request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ChangeVolume endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> CutVideo(
         HttpContext context,
         [FromForm] CutVideoDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                    return Results.BadRequest("Video file is required");
                if (string.IsNullOrEmpty(dto.StartTime) || string.IsNullOrEmpty(dto.EndTime))
                    return Results.BadRequest("Start time and end time are required");

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                try
                {
                    var command = ffmpegService.CreateCutVideoCommand();
                    var result = await command.ExecuteAsync(new CutVideoModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        StartTime = dto.StartTime,
                        EndTime = dto.EndTime
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg cut command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);

                        await fileService.CleanupTempFilesAsync(new List<string> { videoFileName, outputFileName });
                        return Results.Problem("Failed to cut video: " + result.ErrorMessage, statusCode: 500);
                    }

                    string fullOutputPath = fileService.GetFullOutputPath(outputFileName);

                    var fileStream = new FileStream(
                        fullOutputPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 4096,
                        useAsync: true);

                    await fileService.CleanupTempFilesAsync(new List<string> { videoFileName });

                    return Results.File(fileStream, "video/mp4", "cut_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing cut video request");
                    await fileService.CleanupTempFilesAsync(new List<string> { videoFileName, outputFileName });
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CutVideo endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> CreateThumbnail(
        HttpContext context,
        [FromForm] ThumbnailDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                    return Results.BadRequest("Video file is required");
                if (string.IsNullOrEmpty(dto.TimePosition))
                    return Results.BadRequest("Time position is required");

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

                string outputFileName = await fileService.GenerateUniqueFileNameAsync(".jpg");

                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateThumbnailCommand();
                    var result = await command.ExecuteAsync(new ThumbnailModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        TimePosition = dto.TimePosition
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg thumbnail command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);

                        _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                        return Results.Problem("Failed to create thumbnail: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "image/jpeg", Path.GetFileNameWithoutExtension(dto.VideoFile.FileName) + "_thumb.jpg");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing thumbnail request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateThumbnail endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }



        private static async Task<IResult> AdjustBrightnessContrast(
            HttpContext context,
            [FromForm] BrightnessContrastDto dto)
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

                if (dto.Brightness < -1.0 || dto.Brightness > 1.0)
                {
                    return Results.BadRequest("Brightness must be between -1.0 and 1.0");
                }

                if (dto.Contrast < -1000.0 || dto.Contrast > 1000.0)
                {
                    return Results.BadRequest("Contrast must be between -1000.0 and 1000.0");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateBrightnessContrastCommand();
                    var result = await command.ExecuteAsync(new BrightnessContrastModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        Brightness = dto.Brightness,
                        Contrast = dto.Contrast
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg brightness/contrast command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                        return Results.Problem("Failed to adjust brightness/contrast: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    return Results.File(fileBytes, "video/mp4", "adjusted_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing brightness/contrast request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdjustBrightnessContrast endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> RemoveAudio(
    HttpContext context,
    [FromForm] RemoveAudioDto dto)
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

                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateRemoveAudioCommand();

                    var result = await command.ExecuteAsync(new RemoveAudioModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        return Results.Problem(
                            "Failed to remove audio: " + result.ErrorMessage,
                            statusCode: 500);
                    }

                    byte[] fileBytes =
                        await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(
                        fileBytes,
                        "video/mp4",
                        "noaudio_" + dto.VideoFile.FileName);
                }
                catch
                {
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in RemoveAudio endpoint");

                return Results.Problem(
                    "An error occurred: " + ex.Message,
                    statusCode: 500);
            }

        }
        private static async Task<IResult> AddBorder(
            HttpContext context,
            [FromForm] AddBorderDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                    return Results.BadRequest("Video file is required");

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateAddBorderCommand();
                    var result = await command.ExecuteAsync(new AddBorderModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        BorderSize = dto.BorderSize,
                        BorderColor = dto.BorderColor
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg add-border command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                        return Results.Problem("Failed to add border: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    return Results.File(fileBytes, "video/mp4", "bordered_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing add-border request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddBorder endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> BlurVideo(
            HttpContext context,
            [FromForm] BlurVideoDto dto)
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

                if (dto.Sigma <= 0)
                {
                    return Results.BadRequest("Blur strength (sigma) must be greater than 0");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                try
                {
                    // נשלוף את ה-Command בצורה גנרית מה-Factory
                    var command = ffmpegService.CreateBlurVideoCommand();
                    var result = await command.ExecuteAsync(new BlurVideoModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        Sigma = dto.Sigma
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg blur command failed: {ErrorMessage}", result.ErrorMessage);
                        return Results.Problem("Failed to blur video: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", "blurred_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing blur video request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in BlurVideo endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
        private static async Task<IResult> ExtractAudio(
            HttpContext context,
            [FromForm] ExtractAudioDto dto)
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
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(".mp3");

                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateExtractAudioCommand();
                    var result = await command.ExecuteAsync(new ExtractAudioModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg extract audio command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to extract audio: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(
                        fileBytes,
                        "audio/mpeg",
                        Path.GetFileNameWithoutExtension(dto.VideoFile.FileName) + ".mp3");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing extract audio request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ExtractAudio endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
        private static async Task<IResult> CompressVideo(
    HttpContext context,
    [FromForm] VideoCompressionDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                    return Results.BadRequest("Video file is required");

                if (dto.Crf < 0 || dto.Crf > 51)
                    return Results.BadRequest("CRF must be between 0 and 51 (higher = lower quality, 28 is default)");

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateVideoCompressionCommand();
                    var result = await command.ExecuteAsync(new VideoCompressionModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        Crf = dto.Crf
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg compress command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                        return Results.Problem("Failed to compress video: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    return Results.File(fileBytes, "video/mp4", "compressed_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing compress video request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CompressVideo endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
        private static async Task<IResult> ConvertFormat(
    HttpContext context,
    [FromForm] ConvertFormatDto dto)
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
                if (string.IsNullOrWhiteSpace(dto.OutputFormat))
                {
                    return Results.BadRequest("Output format is required (e.g., 'avi', 'mov')");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

                string formatExtension = dto.OutputFormat.StartsWith(".") ? dto.OutputFormat : "." + dto.OutputFormat;
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(formatExtension);

                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateConvertFormatCommand();
                    var result = await command.ExecuteAsync(new ConvertFormatModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg convert format command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to convert video format: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    string originalNameWithoutExtension = Path.GetFileNameWithoutExtension(dto.VideoFile.FileName);
                    return Results.File(fileBytes, "application/octet-stream", originalNameWithoutExtension + formatExtension);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing convert format request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ConvertFormat endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> DuplicateVideo(
            HttpContext context,
            [FromForm] DuplicateVideoDto dto)
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

                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateDuplicateVideoCommand();
                    var result = await command.ExecuteAsync(new DuplicateVideoModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        Duplicates = dto.Duplicates
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg duplicate command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                        return Results.Problem("Failed to duplicate video: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", "duplicated_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing duplicate video request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in DuplicateVideo endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
    }
}


