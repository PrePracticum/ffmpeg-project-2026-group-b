using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class LimitBitrateCommand : BaseCommand, ICommand<LimitBitrateModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public LimitBitrateCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(LimitBitrateModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-b:v {model.Bitrate}") // הוספת ההגבלה כפי שהתבקש במשימה
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}