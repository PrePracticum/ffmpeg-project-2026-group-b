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
    internal class AnimatedTextCommand : BaseCommand, ICommand<AnimatedTextModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        public AnimatedTextCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(AnimatedTextModel model)
        {
            // Build x and y expressions (can be static or animated)
            string xExpression = model.XPosition.ToString();
            string yExpression = model.YPosition.ToString();

            if (model.IsAnimated)
            {
                switch (model.AnimationType)
                {
                    case AnimationType.MoveLeft:
                        // Text moves from right to left across screen
                        xExpression = $"if(lt(t\\,5)\\,w+t*{model.AnimationSpeed}\\,w-50)";
                        break;
                    case AnimationType.MoveRight:
                        // Text moves from left to right
                        xExpression = $"t*{model.AnimationSpeed}";
                        break;
                    case AnimationType.SlideDown:
                        // Text slides down from top
                        yExpression = $"if(lt(t\\,5)\\,0+t*{model.AnimationSpeed}\\,h/2)";
                        break;
                    case AnimationType.SlideUp:
                        // Text slides up from bottom
                        yExpression = $"if(lt(t\\,5)\\,h-t*{model.AnimationSpeed}\\,h/2)";
                        break;
                }
            }

            // Build the drawtext filter with animation support
            // Commas inside expressions must be escaped with backslash
            string drawTextFilter = $"drawtext=text='{model.Text}':x={xExpression}:y={yExpression}:fontsize={model.FontSize}:fontcolor={model.Color}";

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"{drawTextFilter}\"")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
  
}
