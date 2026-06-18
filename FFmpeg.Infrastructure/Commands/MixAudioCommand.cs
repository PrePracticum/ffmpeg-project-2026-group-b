using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class MixAudioCommand : BaseCommand, ICommand<MixAudioModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public MixAudioCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder;
        }

       public async Task<CommandResult> ExecuteAsync(MixAudioModel model)
{
    CommandBuilder = _commandBuilder
        .SetInput(model.InputFile1)
        .SetInput(model.InputFile2)
        .AddFilterComplex("amix=inputs=2[out]")
        .SetOutput(model.OutputFile, isFrameOutput: false);

    return await RunAsync();
}
    }
}