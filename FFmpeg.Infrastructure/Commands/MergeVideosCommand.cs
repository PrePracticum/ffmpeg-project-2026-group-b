using Ffmpeg.Command;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class MergeVideosCommand : BaseCommand, ICommand<MergeVideosModel>
    {
        public MergeVideosCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            CommandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(MergeVideosModel model)
        {
            string filter = model.Mode?.ToLower() == "vertical" ? "vstack" : "hstack";

            string arguments = $"-i \"{model.FirstVideoPath}\" -i \"{model.SecondVideoPath}\" -filter_complex \"[0:v][1:v]{filter}=inputs=2\" \"{model.OutputVideoPath}\"";

            var (success, output, error) = await _executor.RunCommandAsync(arguments);

            return new CommandResult
            {
                IsSuccess = success,
                ErrorMessage = success ? string.Empty : $"Command failed: {error}",
                CommandExecuted = arguments,
                OutputLog = success ? output : error
            };
        }
    }
}