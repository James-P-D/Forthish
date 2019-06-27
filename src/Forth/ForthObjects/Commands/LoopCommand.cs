using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public class LoopCommand : Command
  {
    public LoopCommand()
      : base(Constants.LOOP)
    {
      this.LoopCommands = new List<Command>();
    }

    public List<Command> LoopCommands { get; private set; }
  }
}