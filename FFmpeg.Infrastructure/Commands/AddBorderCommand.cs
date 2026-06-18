using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;

namespace Ffmpeg.Command.Commands
{
    public class AddBorderCommand : BaseCommand, ICommand<AddBorderModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public AddBorderCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(AddBorderModel model)
        {
            int total = model.BorderSize * 2;
            string filter = $"pad=width=iw+{total}:height=ih+{total}:x={model.BorderSize}:y={model.BorderSize}:color={model.BorderColor}";

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"{filter}\"")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}