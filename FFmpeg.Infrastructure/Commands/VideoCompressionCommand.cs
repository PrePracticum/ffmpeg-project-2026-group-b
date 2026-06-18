using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class VideoCompressionCommand : BaseCommand, ICommand<VideoCompressionModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public VideoCompressionCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(VideoCompressionModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .SetVideoCodec("libx264")
                .SetVideoQuality(model.Crf);

            CommandBuilder.SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}