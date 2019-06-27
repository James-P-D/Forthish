using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  /// <summary>
  /// Base Class for Commands.
  /// Most commands will just be treated as strings and executed at run-time. A small number of
  /// special commands ('if..then..else', loops etc.) will require separate objects which include
  /// lists of Command-objects.
  /// </summary>
  public class Command
  {
    public Command(string name)
    {
      this.Name = name;
    }

    public string Name { get; private set; }
  }
}
