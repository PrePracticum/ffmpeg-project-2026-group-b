using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class ReverseVideoCommand : BaseCommand, ICommand<ReverseVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ReverseVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ReverseVideoModel model)
        {
            // בניית פקודת ה-FFmpeg בעזרת ה-Builder
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-vf reverse")  // הוספת פילטר להיפוך הוידאו (Video Filter)
                .AddOption("-af areverse"); // הוספת פילטר להיפוך הסאונד (Audio Filter)

            // הגדרת קובץ הפלט (מכיוון שזה וידאו, נשלח false ל-isFrameOutput)
            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            // הרצת הפקודה באמצעות ה-BaseCommand של הפרויקט
            return await RunAsync();
        }
    
    }
}
