using System.Collections.Generic;

namespace Forth.ForthObjects.Commands
{
    public class RepeatCommand : Command
    {
        public RepeatCommand()
            : base(Constants.Repeat)
        {
            this.RepeatCommands = new List<Command>();
        }

        public List<Command> RepeatCommands { get; }
    }
}