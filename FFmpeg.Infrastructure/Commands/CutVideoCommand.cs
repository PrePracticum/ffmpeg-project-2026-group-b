using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class CutVideoCommand : BaseCommand, ICommand<CutVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public CutVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(CutVideoModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-ss {model.StartTime}")
                .AddOption($"-to {model.EndTime}")
                .AddOption("-c:v libx264")
                .AddOption("-c:a aac")
                .SetOutput(model.OutputFile, false); 

            return await RunAsync();
        }
    }
}