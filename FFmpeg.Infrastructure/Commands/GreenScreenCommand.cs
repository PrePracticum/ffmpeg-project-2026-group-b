using System;
using System.Threading.Tasks;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands; // בשביל BaseCommand ו-ICommand

namespace FFmpeg.Infrastructure.Commands
{
    public class GreenScreenCommand : BaseCommand, ICommand<GreenScreenModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public GreenScreenCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(GreenScreenModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .SetInput(model.BackgroundFile)
                .AddFilterComplex("[0:v]chromakey=0x00FF00:0.1:0.2[ckout];[1:v][ckout]overlay[out]")
                .SetOutput(model.OutputFile); // מחקנו מפה את שורת ה-map!

            return await RunAsync();
        }
    }
}