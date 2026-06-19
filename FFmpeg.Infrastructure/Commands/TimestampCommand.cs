using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    internal class TimestampCommand : BaseCommand, ICommand<TimestampModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public TimestampCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(TimestampModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Build drawtext filter for timestamp using pts:hms
            // Note: escape the ':' in pts\:hms for ffmpeg drawtext
            string drawTextFilter = "drawtext=text='%{pts\\:hms}':x=" + model.XPosition + ":y=" + model.YPosition + 
                ":fontsize=" + model.FontSize + ":fontcolor=" + model.FontColor;

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"{drawTextFilter}\"")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
