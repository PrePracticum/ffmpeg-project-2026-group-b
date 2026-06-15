using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class BrightnessContrastCommand : BaseCommand, ICommand<BrightnessContrastModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public BrightnessContrastCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(BrightnessContrastModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf eq=brightness={model.Brightness}:contrast={model.Contrast}")
                .SetOutput(model.OutputFile, false);

            return await RunAsync();
        }
    }
}
