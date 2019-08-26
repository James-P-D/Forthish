namespace Forth.ForthObjects
{
    public static class Constants
    {
        #region General Constants

        /// <summary>
        /// Default cell size. 4 bytes.
        /// </summary>
        public static int CellSize = 4;

        /// <summary>
        /// Max stack size. 1000 cells not bytes!
        /// </summary>
        public static int MaxStack = 1000;

        /// <summary>
        /// Total size for memory (allocating variables, values, constants)
        /// </summary>
        public static int MemorySize = 1000 * CellSize;

        /// <summary>
        /// Max recursive calls to the actual Forth processor. Calling recursively
        /// (remember, if..then..else, and loops are recursive more than 100 times will 
        /// </summary>
        public static int MaxProcessingStack = 100;

        /// <summary>
        /// In Forth, true is zero
        /// </summary>
        public const int True = 0;

        /// <summary>
        /// In Forth, false is -1
        /// </summary>
        public const int False = -1;

        #endregion

        #region Commands (Reserved Words)

        public const string Period = ".";

        public const string Dup = "dup";
        public const string Swap = "swap";
        public const string Drop = "drop";
        public const string Rot = "rot";
        public const string Over = "over";
        public const string Tuck = "tuck";
        public const string Pick = "pick";
        public const string Roll = "roll";

        public const string Add = "+";
        public const string Subtract = "-";
        public const string Multiply = "*";
        public const string Modulus = "mod";
        public const string Divide = "/";

        public const string GreaterThan = ">";
        public const string LessThan = "<";
        public const string GreaterThanOrEqual = ">=";
        public const string LessThanOrEqual = "<=";
        public const string Equal = "=";
        public const string NotEqual = "!=";
        public const string And = "and";
        public const string Or = "or";
        public const string Not = "not";

        public const string Hex = "hex";
        public const string Decimal = "decimal";
        public const string Char = "char";
        public const string Fractional = "fractional";
        public const string Cr = "cr";

        public const string Variable = "variable";
        public const string Value = "value";
        public const string Constant = "constant";
        public const string To = "to";

        public const string Cell = "cell";
        public const string Here = "here";
        public const string Allot = "allot";

        public const string Store = "!";
        public const string Fetch = "@";

        #region Structured Programming

        public const string Loop = "loop";
        public const string EndLoop = "endloop";

        public const string Repeat = "repeat";
        public const string Until = "until";

        public const string If = "if";
        public const string Else = "else";
        public const string EndIf = "endif";

        public const string ViewDefinitions = "viewdefinitions";
        public const string ViewObjects = "viewobjects";
        public const string Help = "help";

        #endregion

        #region Syntactic

        public const string StartDefinition = ":";
        public const string RedefineDefinition = "::";
        public const string EndDefinition = ";";

        public const string StartString = ".\"";
        public const string EndString = "\"";

        public const string StartComment = "(";
        public const string EndComment = ")";

        public const string StartChar = "'";
        public const string EndChar = "'";

        public const string Continuation = "\\";

        #endregion

        #endregion

        public static string[] ValidCommands =
        {
            Period,

            Dup,
            Swap,
            Drop,
            Rot,
            Over,
            Tuck,
            Pick,
            Roll,

            Add,
            Subtract,
            Multiply,
            Divide,
            Modulus,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
            Equal,
            NotEqual,
            And,
            Or,
            Not,

            Hex,
            Decimal,
            Char,
            Cr,

            Variable,
            Value,
            Constant,
            To,

            Store,
            Fetch,

            Cell,
            Here,

            Repeat,
            Until,

            Loop,
            EndLoop,

            If,
            Else,
            EndIf,

            ViewDefinitions,
            ViewObjects,
            Help
        };
    }
}