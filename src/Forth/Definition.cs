using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public class Definition
  {
    public Definition(string name)
    {
      this.Name = name;
      this.Parameters = new List<string>();
    }

    public void AddParameter(string parameter)
    {
      this.Parameters.Add(parameter);
    }

    public string Name { get; private set; }

    public List<string> Parameters {get;private set;}
  }
}
