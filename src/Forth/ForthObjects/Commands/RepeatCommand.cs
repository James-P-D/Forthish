using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public class RepeatCommand : Command
  {
    public RepeatCommand()
      : base(Constants.REPEAT)
    {
      this.RepeatCommands = new List<Command>();
    }

    public List<Command> RepeatCommands { get; private set; }
  }
}