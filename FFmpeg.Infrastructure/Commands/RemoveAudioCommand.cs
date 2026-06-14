using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;

namespace FFmpeg.Infrastructure.Commands
{
    public class RemoveAudioCommand : BaseCommand, ICommand<RemoveAudioModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public RemoveAudioCommand(
            FFmpegExecutor executor,
            ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ??
                throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(
            RemoveAudioModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-an");

            CommandBuilder.SetOutput(
                model.OutputFile,
                isFrameOutput: false);

            return await RunAsync();
        }
    }
}