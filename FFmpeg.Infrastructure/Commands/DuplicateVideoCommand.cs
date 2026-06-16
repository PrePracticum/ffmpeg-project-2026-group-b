using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;
using Ffmpeg.Command.Commands;

namespace FFmpeg.Infrastructure.Commands
{
    public class DuplicateVideoCommand : BaseCommand, ICommand<DuplicateVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public DuplicateVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ??
                throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(DuplicateVideoModel model)
        {
            int duplicates = model.Duplicates > 1 ? model.Duplicates : 2;

            // יצירת תוויות (Pads) לפיצול הוידאו. למשל אם שולחים 2, נקבל [v0][v1]
            string splitPads = "";
            for (int i = 0; i < duplicates; i++)
            {
                splitPads += $"[v{i}]";
            }

            // בניית פקודת הפילטר המלאה (קודם מפצלים את הוידאו, ואז מחברים אותם לרוחב)
            // דוגמה לתוצר סופי: [0:v]split=2[v0][v1];[v0][v1]hstack=inputs=2[out]
            string filterComplex = $"[0:v]split={duplicates}{splitPads};{splitPads}hstack=inputs={duplicates}[out]";

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-filter_complex \"{filterComplex}\"")
                .AddOption("-map \"[out]\"")
                .AddOption("-map 0:a?"); // שומר על פס הקול המקורי של הסרטון (אופציונלי אך מומלץ)

            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            return await RunAsync();
        }
    }
}
