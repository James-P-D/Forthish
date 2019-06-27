using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public class FFloat
  {
    public FFloat(float value)
    {
      this.Value = value;
    }

    public float Value { get; set; }

    public byte[] GetBytes()
    {
      return BitConverter.GetBytes(this.Value);
    }
  }
}
