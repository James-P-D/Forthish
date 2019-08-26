using System;
using System.Collections.Generic;

namespace Forth.ForthObjects
{
    /// <summary>
    /// Forth Stack
    /// </summary>
    public class FStack
    {
        /// <summary>
        /// The actual Stack of Cell objects
        /// </summary>
        private readonly Stack<Cell> _stack;

        #region Constructor

        /// <summary>
        /// Constructor. Simply initialise our Stack
        /// </summary>
        public FStack()
        {
            this._stack = new Stack<Cell>();
        }

        #endregion

        #region Push/Pop

        /// <summary>
        /// Push a Cell to the Stack
        /// </summary>
        /// <param name="cell">Cell to push</param>
        public void Push(Cell cell)
        {
            if (this._stack.Count >= Constants.MaxStack)
            {
                throw new Exception(Resources.StackOverflow);
            }

            this._stack.Push(cell);
        }

        /// <summary>
        /// Test to see if we can Pop a Cell
        /// </summary>
        /// <returns>True if Stack is empty, False otherwise</returns>
        public bool CanPop()
        {
            return this._stack.Count > 0;
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

            return this._stack.Pop();
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
            FInteger index = new FInteger(BitConverter.ToInt32(_stack.Pop().Bytes, 0));
            Stack<Cell> savedCells = new Stack<Cell>();
            for (int i = 0; i < index.Value; i++)
            {
                savedCells.Push(this.Pop());
            }

            Cell pickedCell = this.Pop();
            this.Push(pickedCell);

            while (savedCells.Count > 0)
            {

                this.Push(savedCells.Pop());
            }

            this.Push(pickedCell);
        }

        public void Roll()
        {
            FInteger index = new FInteger(BitConverter.ToInt32(_stack.Pop().Bytes, 0));
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
            FInteger intConstant1 = new FInteger(BitConverter.ToInt32(_stack.Pop().Bytes, 0));
            if (token.Equals(Constants.Not))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value != Constants.True ? Constants.True : Constants.False).GetBytes()));
                return;
            }

            FInteger intConstant2 = new FInteger(BitConverter.ToInt32(_stack.Pop().Bytes, 0));

            if (token.Equals(Constants.Add))
            {
                _stack.Push(new Cell(new FInteger(intConstant1.Value + intConstant2.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Subtract))
            {
                _stack.Push(new Cell(new FInteger(intConstant2.Value - intConstant1.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Multiply))
            {
                _stack.Push(new Cell(new FInteger(intConstant2.Value * intConstant1.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Divide))
            {
                _stack.Push(new Cell(new FInteger(intConstant2.Value / intConstant1.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Modulus))
            {
                _stack.Push(new Cell(new FInteger(intConstant2.Value % intConstant1.Value).GetBytes()));
            }
            else if (token.Equals(Constants.GreaterThan))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value > intConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.GreaterThanOrEqual))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value >= intConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.LessThan))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value < intConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.LessThanOrEqual))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value <= intConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.Equal))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value == intConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.NotEqual))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value != intConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.And))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value == Constants.True && intConstant2.Value == Constants.True
                        ? Constants.True
                        : Constants.False).GetBytes()));
            }
            else if (token.Equals(Constants.Or))
            {
                _stack.Push(new Cell(
                    new FInteger(intConstant1.Value == Constants.True || intConstant2.Value == Constants.True
                        ? Constants.True
                        : Constants.False).GetBytes()));
            }
        }

        /// <summary>
        /// Performs a mathematical function. Most functions will pop two items from the stack, perform the operation
        /// and push the result
        /// </summary>
        /// <param name="token">String containing the mathematical operator</param>
        public void FloatMaths(string token)
        {
            FFloat floatConstant1 = new FFloat(BitConverter.ToSingle(_stack.Pop().Bytes, 0));
            FFloat floatConstant2 = new FFloat(BitConverter.ToSingle(_stack.Pop().Bytes, 0));

            if (token.Equals(Constants.Add))
            {
                _stack.Push(new Cell(new FFloat(floatConstant1.Value + floatConstant2.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Subtract))
            {
                _stack.Push(new Cell(new FFloat(floatConstant1.Value - floatConstant2.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Multiply))
            {
                _stack.Push(new Cell(new FFloat(floatConstant2.Value * floatConstant1.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Divide))
            {
                _stack.Push(new Cell(new FFloat(floatConstant2.Value / floatConstant1.Value).GetBytes()));
            }
            else if (token.Equals(Constants.Modulus))
            {
                _stack.Push(new Cell(new FFloat(floatConstant2.Value % floatConstant1.Value).GetBytes()));
            }
            else if (token.Equals(Constants.GreaterThan))
            {
                _stack.Push(new Cell(
                    new FFloat(floatConstant1.Value > floatConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.GreaterThanOrEqual))
            {
                _stack.Push(new Cell(
                    new FFloat(floatConstant1.Value >= floatConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.LessThan))
            {
                _stack.Push(new Cell(
                    new FFloat(floatConstant1.Value < floatConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.LessThanOrEqual))
            {
                _stack.Push(new Cell(
                    new FFloat(floatConstant1.Value <= floatConstant2.Value ? Constants.True : Constants.False)
                        .GetBytes()));
            }
            else if (token.Equals(Constants.Equal))
            {
                double tolerance = 0.0001;
                _stack.Push(new Cell(new FFloat(Math.Abs(floatConstant1.Value - floatConstant2.Value) < tolerance
                    ? Constants.True
                    : Constants.False).GetBytes()));
            }
            else if (token.Equals(Constants.NotEqual))
            {
                double tolerance = 0.0001;
                _stack.Push(new Cell(new FFloat(Math.Abs(floatConstant1.Value - floatConstant2.Value) > tolerance
                    ? Constants.True
                    : Constants.False).GetBytes()));
            }
        }

        #endregion
    }
}