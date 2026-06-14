using Ffmpeg.Command;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FFmpeg.Infrastructure.Services
{
    public interface IFFmpegServiceFactory
    {
        ICommand<WatermarkModel> CreateWatermarkCommand();
        ICommand<ReverseVideoModel> CreateReverseVideoCommand();
        ICommand<ChangeAudioFormatModel> CreateChangeAudioFormatCommand();
        ICommand<AnimatedTextModel> CreateAnimatedTextCommand();
        ICommand<CutVideoModel> CreateCutVideoCommand();
        ICommand<GreenScreenModel> CreateGreenScreenCommand();
      
        ICommand<SubtitlesModel> CreateSubtitlesCommand();



        ICommand<ChangeResolutionModel> CreateChangeResolutionCommand();
        
        ICommand<CropVideoModel> CreateCropVideoCommand();

        ICommand<ChangeSpeedModel> CreateChangeSpeedCommand();

        ICommand<ThumbnailModel> CreateThumbnailCommand();
    }

    public class FFmpegServiceFactory : IFFmpegServiceFactory
    {
        private readonly FFmpegExecutor _executor;
        private readonly ICommandBuilder _commandBuilder;

        public FFmpegServiceFactory(IConfiguration configuration, ILogger logger = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string ffmpegPath = Path.Combine(baseDirectory, "external", "ffmpeg.exe");

            bool logOutput = bool.TryParse(configuration["FFmpeg:LogOutput"], out bool log) && log;

            _executor = new FFmpegExecutor(ffmpegPath, logOutput, logger);
            _commandBuilder = new CommandBuilder(configuration);
        }

        public ICommand<WatermarkModel> CreateWatermarkCommand()
        {
            return new WatermarkCommand(_executor, _commandBuilder);
        }

        public ICommand<ReverseVideoModel> CreateReverseVideoCommand()
        {
            return new ReverseVideoCommand(_executor, _commandBuilder);
        }

        public ICommand<ChangeAudioFormatModel> CreateChangeAudioFormatCommand()
        {
            return new ChangeAudioFormatCommand(_executor);
        public ICommand<AnimatedTextModel> CreateAnimatedTextCommand()
        {
            return new AnimatedTextCommand(_executor, _commandBuilder);
        }

        public ICommand<GreenScreenModel> CreateGreenScreenCommand()
        {
            return new GreenScreenCommand(_executor, _commandBuilder);
        }

        public ICommand<CutVideoModel> CreateCutVideoCommand()
        {
            return new CutVideoCommand(_executor, _commandBuilder);
        }

        public ICommand<SubtitlesModel> CreateSubtitlesCommand()
        {
            return new SubtitlesCommand(_executor, _commandBuilder);
         }


        public ICommand<ChangeResolutionModel> CreateChangeResolutionCommand()
        {
            return new ChangeResolutionCommand(_executor, _commandBuilder);
        }

        public ICommand<CropVideoModel> CreateCropVideoCommand()
        {
            return new CropVideoCommand(_executor, _commandBuilder);
        }

        public ICommand<ChangeSpeedModel> CreateChangeSpeedCommand()
        {
            return new ChangeSpeedCommand(_executor, _commandBuilder);

        }

        public ICommand<ThumbnailModel> CreateThumbnailCommand()
        {
            return new ThumbnailCommand(_executor, _commandBuilder);
        }
    }
}