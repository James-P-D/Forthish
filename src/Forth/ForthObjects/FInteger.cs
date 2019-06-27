using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public class FInteger
  {
    public FInteger(Int32 value)
    {
      this.Value = value;
    }

    public Int32 Value { get; set; }

    public byte[] GetBytes()
    {
      return BitConverter.GetBytes(this.Value);
    }
  }
}