using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command.Commands;
using System;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class ExtractAudioCommand : BaseCommand, ICommand<ExtractAudioModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ExtractAudioCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ??
                throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ExtractAudioModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-q:a 0")
                .AddOption("-map a");

            CommandBuilder.SetOutput(model.OutputFile, isFrameOutput: false);

            return await RunAsync();
        }
    }
}
