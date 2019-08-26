using System;

namespace Forth.ForthObjects
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