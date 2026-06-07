using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands; 

namespace FFmpeg.Infrastructure.Commands
{
    public class ChangeSpeedCommand : BaseCommand, ICommand<ChangeSpeedModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ChangeSpeedCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ChangeSpeedModel model)
        {
            if (model.SpeedMultiplier <= 0)
            {
                throw new ArgumentException("Speed multiplier must be greater than zero.", nameof(model.SpeedMultiplier));
            }

           
            double setptsValue = 1.0 / model.SpeedMultiplier;

           
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"setpts={setptsValue:0.0###}*PTS\"");

            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            return await RunAsync();
        }
    }
}
