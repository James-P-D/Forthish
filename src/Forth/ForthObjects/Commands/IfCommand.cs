using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public class IfCommand : Command
  {
    /// <summary>
    /// If Command
    /// </summary>
    public IfCommand()
      : base(Constants.IF)
    {
      this.IfCommands = new List<Command>();
      this.ElseCommands = new List<Command>();
    }
        
    public List<Command> IfCommands { get; private set; }

    public List<Command> ElseCommands { get; private set; }
  }
}
