using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands;
using System;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class ReplaceAudioCommand : BaseCommand, ICommand<ReplaceAudioModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ReplaceAudioCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ??
                throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ReplaceAudioModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputVideoFile)
                .SetInput(model.NewAudioFile)
                .AddOption("-c:v copy")
                .AddOption("-map 0:v:0")
                .AddOption("-map 1:a:0");

            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            return await RunAsync();
        }
    }
}
