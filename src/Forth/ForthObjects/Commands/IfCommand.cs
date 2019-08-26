using System.Collections.Generic;

namespace Forth.ForthObjects.Commands
{
    public class IfCommand : Command
    {
        /// <summary>
        /// If Command
        /// </summary>
        public IfCommand()
            : base(Constants.If)
        {
            this.IfCommands = new List<Command>();
            this.ElseCommands = new List<Command>();
        }

        public List<Command> IfCommands { get; }

        public List<Command> ElseCommands { get; }
    }
}