using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class ChangeResolutionCommand : BaseCommand, ICommand<ChangeResolutionModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ChangeResolutionCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ChangeResolutionModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                // שינוי המידות + format למניעת מסך שחור + setdar=0 שמבטל את המתיחה של הנגן
                .AddOption($"-vf [0:v]scale={model.Width}:{model.Height}[v];[v]format=yuv420p,setdar=0")
                .AddOption("-c:v libx264")
                .AddOption("-c:a copy");

            CommandBuilder.SetOutput(model.OutputFile, false);

            return await RunAsync();
        }
    }
}