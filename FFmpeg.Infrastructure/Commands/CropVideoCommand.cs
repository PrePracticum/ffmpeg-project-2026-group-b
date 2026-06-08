using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;

namespace FFmpeg.Infrastructure.Commands
{
    public class CropVideoCommand : BaseCommand, ICommand<CropVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public CropVideoCommand(
            FFmpegExecutor executor,
            ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder;
        }

        public async Task<CommandResult> ExecuteAsync(CropVideoModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption(
                    $"-vf \"crop={model.Width}:{model.Height}:{model.X}:{model.Y}\"");

            CommandBuilder.SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
