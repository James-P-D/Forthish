using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  /// <summary>
  /// Basic unit in Forth. All variables will be one-cell in size (obviously
  /// arrays can be N-cells in size, memory depending.)
  /// </summary>
  public class Cell
  {
    /// <summary>
    /// Constructor. Initialise the byte array to our given Cell size
    /// </summary>
    public Cell()
    {
      this.Bytes = new byte[Constants.CELL_SIZE];
    }

    /// <summary>
    /// Constructor. Initialise the byte array to given input.
    /// </summary>
    /// <param name="bytes">Initial bytes for Cell</param>
    public Cell(byte[] bytes)
    {
      if (bytes.Length != Constants.CELL_SIZE)
      {
        throw new Exception(Resources.WrongCellSize);
      }

      this.Bytes = new byte[Constants.CELL_SIZE];
      Array.Copy(bytes, this.Bytes, bytes.Length);
    }

    /// <summary>
    /// Public property for our actual byte array
    /// </summary>
    public byte[] Bytes
    {
      get;
      set;
    }
  }
}