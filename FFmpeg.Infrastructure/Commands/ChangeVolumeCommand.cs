using System;
using System.Threading.Tasks;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands;

namespace FFmpeg.Infrastructure.Commands
{
    public class ChangeVolumeCommand : BaseCommand, ICommand<ChangeVolumeModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ChangeVolumeCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ChangeVolumeModel model)
        {
            if (model.VolumeMultiplier <= 0)
            {
                throw new ArgumentException("Volume multiplier must be greater than zero.", nameof(model.VolumeMultiplier));
            }

            // ffmpeg -i input.mp4 -af "volume=2.0" output.mp4
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-af \"volume={model.VolumeMultiplier:0.###}\"");

            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            return await RunAsync();
        }
    }
}
