using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  /// <summary>
  /// Forth Stack
  /// </summary>
  public class FStack
  {
    /// <summary>
    /// The actual Stack of Cell objects
    /// </summary>
    private Stack<Cell> Stack;

    #region Constructor

    /// <summary>
    /// Constructor. Simply initialise our Stack
    /// </summary>
    public FStack()
    {
      this.Stack = new Stack<Cell>();
    }

    #endregion

    #region Push/Pop

    /// <summary>
    /// Push a Cell to the Stack
    /// </summary>
    /// <param name="cell">Cell to push</param>
    public void Push(Cell cell)
    {
      if (this.Stack.Count() >= Constants.MAX_STACK)
      {
        throw new Exception(Resources.StackOverflow);
      }
      this.Stack.Push(cell);
    }

    /// <summary>
    /// Test to see if we can Pop a Cell
    /// </summary>
    /// <returns>True if Stack is empty, False otherwise</returns>
    public bool CanPop()
    {
      return this.Stack.Count() > 0;
    }

    /// <summary>
    /// Pops a Cell from the Stack
    /// </summary>
    /// <returns>The top Cell on the Stack</returns>
    public Cell Pop()
    {
      if (!this.CanPop())
      {
        throw new Exception(Resources.StackUnderflow);
      }
      return this.Stack.Pop();
    }

    #endregion

    #region Additional Stack Functions

    /// <summary>
    /// Duplicate top of stack
    /// </summary>
    public void Dup()
    {
      Cell cell = this.Pop();
      Cell cellDuplicate = new Cell(cell.Bytes);
      this.Push(cell);
      this.Push(cellDuplicate);
    }

    /// <summary>
    /// Swap top two items on stack
    /// </summary>
    public void Swap()
    {
      Cell cell1 = this.Pop();
      Cell cell2 = this.Pop();
      this.Push(cell1);
      this.Push(cell2);
    }

    /// <summary>
    /// Discard the top item on the stack
    /// </summary>
    public void Drop()
    {
      this.Pop();
    }

    public void Rot()
    {
      Cell cell1 = this.Pop();
      Cell cell2 = this.Pop();
      Cell cell3 = this.Pop();
      this.Push(cell2);
      this.Push(cell1);
      this.Push(cell3);
    }

    public void Over()
    {
      Cell cell1 = this.Pop();
      Cell cell2 = this.Pop();
      Cell cell3 = new Cell(cell2.Bytes);
      this.Push(cell3);
      this.Push(cell1);
      this.Push(cell2);
    }

    public void Tuck()
    {
      Cell cell1 = this.Pop();
      Cell cell2 = new Cell(cell1.Bytes);
      Cell cell3 = this.Pop();
      this.Push(cell2);
      this.Push(cell3);
      this.Push(cell1);
    }

    public void Pick()
    {
      FInteger index = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
      Stack<Cell> savedCells = new Stack<Cell>();
      for (int i = 0; i < index.Value; i++)
      {
        savedCells.Push(this.Pop());
      }

      Cell pickedCell = this.Pop();
      Cell duplicateCell = new Cell(pickedCell.Bytes);

      this.Push(pickedCell);

      while (savedCells.Count > 0)
      {

        this.Push(savedCells.Pop());
      }

      this.Push(pickedCell);
    }

    public void Roll()
    {
      FInteger index = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
      Stack<Cell> savedCells = new Stack<Cell>();
      for (int i = 0; i < index.Value; i++)
      {
        savedCells.Push(this.Pop());
      }

      Cell rolledCell = this.Pop();

      while (savedCells.Count > 0)
      {

        this.Push(savedCells.Pop());
      }

      this.Push(rolledCell);
    }

    #endregion

    #region Maths

    /// <summary>
    /// Performs a mathematical function. Most functions will pop two items from the stack, perform the operation
    /// and push the result
    /// </summary>
    /// <param name="token">String containing the mathematical operator</param>
    public void IntMaths(string token)
    {
      FInteger intConstant1 = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
      if (token.Equals(Constants.NOT))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value != Constants.TRUE ? Constants.TRUE : Constants.FALSE).GetBytes()));
        return;
      }

      FInteger intConstant2 = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));

      if (token.Equals(Constants.ADD))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value + intConstant2.Value).GetBytes()));
      }
      else if (token.Equals(Constants.SUBTRACT))
      {
        Stack.Push(new Cell(new FInteger(intConstant2.Value - intConstant1.Value).GetBytes()));
      }
      else if (token.Equals(Constants.MULTIPLY))
      {
        Stack.Push(new Cell(new FInteger(intConstant2.Value * intConstant1.Value).GetBytes()));
      }
      else if (token.Equals(Constants.DIVIDE))
      {
        Stack.Push(new Cell(new FInteger(intConstant2.Value / intConstant1.Value).GetBytes()));
      }
      else if (token.Equals(Constants.MODULUS))
      {
        Stack.Push(new Cell(new FInteger(intConstant2.Value % intConstant1.Value).GetBytes()));
      }
      else if (token.Equals(Constants.GREATER_THAN))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value > intConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.GREATER_THAN_OR_EQUAL))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value >= intConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.LESS_THAN))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value < intConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.LESS_THAN_OR_EQUAL))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value <= intConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.EQUAL))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value == intConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.NOT_EQUAL))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value != intConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.AND))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value == Constants.TRUE && intConstant2.Value == Constants.TRUE ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.OR))
      {
        Stack.Push(new Cell(new FInteger(intConstant1.Value == Constants.TRUE || intConstant2.Value == Constants.TRUE ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
    }

    /// <summary>
    /// Performs a mathematical function. Most functions will pop two items from the stack, perform the operation
    /// and push the result
    /// </summary>
    /// <param name="token">String containing the mathematical operator</param>
    public void FloatMaths(string token)
    {
      FFloat floatConstant1 = new FFloat(BitConverter.ToSingle(Stack.Pop().Bytes, 0));
      FFloat floatConstant2 = new FFloat(BitConverter.ToSingle(Stack.Pop().Bytes, 0));

      if (token.Equals(Constants.ADD))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value + floatConstant2.Value).GetBytes()));
      }
      else if (token.Equals(Constants.SUBTRACT))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value - floatConstant2.Value).GetBytes()));
      }
      else if (token.Equals(Constants.MULTIPLY))
      {
        Stack.Push(new Cell(new FFloat(floatConstant2.Value * floatConstant1.Value).GetBytes()));
      }
      else if (token.Equals(Constants.DIVIDE))
      {
        Stack.Push(new Cell(new FFloat(floatConstant2.Value / floatConstant1.Value).GetBytes()));
      }
      else if (token.Equals(Constants.MODULUS))
      {
        Stack.Push(new Cell(new FFloat(floatConstant2.Value % floatConstant1.Value).GetBytes()));
      }
      else if (token.Equals(Constants.GREATER_THAN))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value > floatConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.GREATER_THAN_OR_EQUAL))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value >= floatConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.LESS_THAN))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value < floatConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.LESS_THAN_OR_EQUAL))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value <= floatConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.EQUAL))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value == floatConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
      else if (token.Equals(Constants.NOT_EQUAL))
      {
        Stack.Push(new Cell(new FFloat(floatConstant1.Value != floatConstant2.Value ? Constants.TRUE : Constants.FALSE).GetBytes()));
      }
    }

    #endregion
  }
}