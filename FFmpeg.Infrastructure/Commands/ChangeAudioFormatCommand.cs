using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class ChangeAudioFormatCommand : ICommand<ChangeAudioFormatModel>
    {
        private readonly FFmpegExecutor _executor;

        public ChangeAudioFormatCommand(FFmpegExecutor executor)
        {
            _executor = executor;
        }

        public async Task<CommandResult> ExecuteAsync(ChangeAudioFormatModel model)
        {
            // פקודת ה-ffmpeg המדויקת מהמשימה שלך
            string arguments = $"-i \"{model.InputFile}\" \"{model.OutputFile}\"";
            
            var (success, output, error) = await _executor.RunCommandAsync(arguments);
            
            return new CommandResult 
            { 
                IsSuccess = success, 
                ErrorMessage = error,
                CommandExecuted = arguments
            };
        }
    }
}
