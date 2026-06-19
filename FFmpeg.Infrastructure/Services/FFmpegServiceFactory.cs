using Ffmpeg.Command;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        ICommand<ExtractFrameModel> CreateExtractFrameCommand();
        ICommand<SubtitlesModel> CreateSubtitlesCommand();
        ICommand<ChangeResolutionModel> CreateChangeResolutionCommand();
        ICommand<CropVideoModel> CreateCropVideoCommand();
        ICommand<ChangeSpeedModel> CreateChangeSpeedCommand();
    ICommand<ChangeVolumeModel> CreateChangeVolumeCommand();

        ICommand<ThumbnailModel> CreateThumbnailCommand();
        ICommand<RemoveAudioModel> CreateRemoveAudioCommand();

        ICommand<ConvertFormatModel> CreateConvertFormatCommand();
        ICommand<ExtractAudioModel> CreateExtractAudioCommand();
        ICommand<BrightnessContrastModel> CreateBrightnessContrastCommand();
        ICommand<AddBorderModel> CreateAddBorderCommand();
        ICommand<BlurVideoModel> CreateBlurVideoCommand();
        ICommand<VideoCompressionModel> CreateVideoCompressionCommand();
        ICommand<DuplicateVideoModel> CreateDuplicateVideoCommand();
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
        }
        public ICommand<AnimatedTextModel> CreateAnimatedTextCommand()
        {
            return new AnimatedTextCommand(_executor, _commandBuilder);
        }
        public ICommand<GreenScreenModel> CreateGreenScreenCommand()
        {
            return new GreenScreenCommand(_executor, _commandBuilder);
        }
        public ICommand<ExtractFrameModel> CreateExtractFrameCommand()
        {
            return new ExtractFrameCommand(_executor, _commandBuilder);
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

        public ICommand<ChangeVolumeModel> CreateChangeVolumeCommand()
        {
            return new ChangeVolumeCommand(_executor, _commandBuilder);
        }

        public ICommand<ThumbnailModel> CreateThumbnailCommand()
        {
            return new ThumbnailCommand(_executor, _commandBuilder);
        }

        public ICommand<RemoveAudioModel> CreateRemoveAudioCommand()
        {
            return new RemoveAudioCommand(_executor, _commandBuilder);
        }

        public ICommand<ConvertFormatModel> CreateConvertFormatCommand()
        {
            return new ConvertFormatCommand(_executor, _commandBuilder);
        }
        public ICommand<ExtractAudioModel> CreateExtractAudioCommand()
        {
            return new ExtractAudioCommand(_executor, _commandBuilder);
        }
        public ICommand<BrightnessContrastModel> CreateBrightnessContrastCommand()
        {
            return new BrightnessContrastCommand(_executor, _commandBuilder);
        }

        public ICommand<AddBorderModel> CreateAddBorderCommand()
        {
            return new AddBorderCommand(_executor, _commandBuilder);
        }

        public ICommand<BlurVideoModel> CreateBlurVideoCommand()
        {
            return new BlurVideoCommand(_executor, _commandBuilder);
        }
        public ICommand<VideoCompressionModel> CreateVideoCompressionCommand()
        {
            return new VideoCompressionCommand(_executor, _commandBuilder);
        }
        public ICommand<DuplicateVideoModel> CreateDuplicateVideoCommand()
        {
            return new DuplicateVideoCommand(_executor, _commandBuilder());
        }
    }
}