using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class ThumbnailCommand : BaseCommand, ICommand<ThumbnailModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ThumbnailCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ThumbnailModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-ss {model.TimePosition}")
                .SetOutput(model.OutputFile, isFrameOutput: true, frameCount: 1);

            return await RunAsync();
        }
    }
}