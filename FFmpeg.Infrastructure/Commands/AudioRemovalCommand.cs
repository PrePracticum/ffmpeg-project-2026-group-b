using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class AudioRemovalCommand : BaseCommand, ICommand<AudioRemovalModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public AudioRemovalCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(AudioRemovalModel model)
        {
            // בניית פקודת ה-FFmpeg להסרת אודיו באמצעות ה-Builder של הפרויקט שלך
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-an");

            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            // הרצת הפקודה דרך מחלקת האב BaseCommand
            return await RunAsync();
        }
    }
}