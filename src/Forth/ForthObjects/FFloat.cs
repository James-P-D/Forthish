using System;

namespace Forth.ForthObjects
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