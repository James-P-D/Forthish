using System.Collections.Generic;

namespace Forth.ForthObjects.Commands
{
    public class LoopCommand : Command
    {
        public LoopCommand()
            : base(Constants.Loop)
        {
            this.LoopCommands = new List<Command>();
        }

        public List<Command> LoopCommands { get; }
    }
}