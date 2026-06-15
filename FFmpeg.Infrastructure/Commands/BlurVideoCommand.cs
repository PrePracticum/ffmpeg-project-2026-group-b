using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class BlurVideoCommand : BaseCommand, ICommand<BlurVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        // תיקון: מעבירים ל-base רק את ה-executor, כי זה מה שהוא מצפה לקבל
        public BlurVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(BlurVideoModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"gblur=sigma={model.Sigma}\"");

            CommandBuilder.SetOutput(model.OutputFile, false);

            return await RunAsync();
        }
    }
}