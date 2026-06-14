using System;
using System.Threading.Tasks;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands;

namespace FFmpeg.Infrastructure.Commands
{
    public class ConvertFormatCommand : BaseCommand, ICommand<ConvertFormatModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ConvertFormatCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ConvertFormatModel model)
        {
            CommandBuilder = _commandBuilder.SetInput(model.InputFile);

            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            return await RunAsync();
        }
    }
}