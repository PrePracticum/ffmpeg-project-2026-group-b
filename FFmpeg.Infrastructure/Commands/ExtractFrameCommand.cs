using System;
using System.Threading.Tasks;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands;

namespace FFmpeg.Infrastructure.Commands
{
    public class ExtractFrameCommand : BaseCommand, ICommand<ExtractFrameModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ExtractFrameCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ExtractFrameModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-ss {model.TimeStamp}")
                .SetOutput(model.OutputFile, true, 1);

            return await RunAsync();
        }
    }
}