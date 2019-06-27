using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public static class Constants
  {
    #region General Constants

    /// <summary>
    /// Default cell size. 4 bytes.
    /// </summary>
    public static int CELL_SIZE = 4;

    /// <summary>
    /// Max stack size. 1000 cells not bytes!
    /// </summary>
    public static int MAX_STACK = 1000;

    /// <summary>
    /// Total size for memory (allocating variables, values, constants)
    /// </summary>
    public static int MEMORY_SIZE = 1000 * CELL_SIZE;

    /// <summary>
    /// Max recursive calls to the actual Forth processor. Calling recursively
    /// (remember, if..then..else, and loops are recursive more than 100 times will 
    /// </summary>
    public static int MAX_PROCESSING_STACK = 100;

    /// <summary>
    /// In Forth, true is zero
    /// </summary>
    public const int TRUE = 0;

    /// <summary>
    /// In Forth, false is -1
    /// </summary>
    public const int FALSE = -1;

    #endregion

    #region Commands (Reserved Words)

    public const string PERIOD = ".";

    public const string DUP = "dup";
    public const string SWAP = "swap";
    public const string DROP = "drop";
    public const string ROT = "rot";
    public const string OVER = "over";
    public const string TUCK = "tuck";
    public const string PICK = "pick";
    public const string ROLL = "roll";

    public const string ADD = "+";
    public const string SUBTRACT = "-";
    public const string MULTIPLY = "*";
    public const string MODULUS = "mod";
    public const string DIVIDE = "/";

    public const string GREATER_THAN = ">";
    public const string LESS_THAN = "<";
    public const string GREATER_THAN_OR_EQUAL = ">=";
    public const string LESS_THAN_OR_EQUAL = "<=";
    public const string EQUAL = "=";
    public const string NOT_EQUAL = "!=";
    public const string AND = "and";
    public const string OR = "or";
    public const string NOT = "not";

    public const string HEX = "hex";
    public const string DECIMAL = "decimal";
    public const string CHAR = "char";
    public const string FRACTIONAL = "fractional";
    public const string CR = "cr";

    public const string VARIABLE = "variable";
    public const string VALUE = "value";
    public const string CONSTANT = "constant";
    public const string TO = "to";

    public const string CELL = "cell";
    public const string HERE = "here";
    public const string ALLOT = "allot";

    public const string STORE = "!";
    public const string FETCH = "@";

    #region Structured Programming

    public const string LOOP = "loop";
    public const string END_LOOP = "endloop";

    public const string REPEAT = "repeat";
    public const string UNTIL = "until";

    public const string IF = "if";
    public const string ELSE = "else";
    public const string END_IF = "endif";

    public const string VIEW_DEFINITIONS = "viewdefinitions";
    public const string VIEW_OBJECTS = "viewobjects";
    public const string HELP = "help";

    #endregion

    #region Syntactic

    public const string START_DEFINITION = ":";
    public const string REDEFINE_DEFINITION = "::";
    public const string END_DEFINITION = ";";

    public const string START_STRING = ".\"";
    public const string END_STRING = "\"";

    public const string START_COMMENT = "(";
    public const string END_COMMENT = ")";

    public const string START_CHAR = "'";
    public const string END_CHAR = "'";

    public const string CONTINUATION = "\\";

    #endregion

    #endregion

    public static string[] VALID_COMMANDS = {
                                             PERIOD,

                                             DUP,
                                             SWAP,
                                             DROP,
                                             ROT,
                                             OVER,
                                             TUCK,
                                             PICK,
                                             ROLL,
                                             
                                             ADD,
                                             SUBTRACT,
                                             MULTIPLY,
                                             DIVIDE,
                                             MODULUS,
                                             GREATER_THAN,
                                             LESS_THAN,
                                             GREATER_THAN_OR_EQUAL,
                                             LESS_THAN_OR_EQUAL,
                                             EQUAL,
                                             NOT_EQUAL,
                                             AND,
                                             OR,
                                             NOT,
                                      
                                             HEX,
                                             DECIMAL,
                                             CHAR,
                                             CR,

                                             VARIABLE,
                                             VALUE,
                                             CONSTANT,
                                             TO,

                                             STORE,
                                             FETCH,

                                             CELL,
                                             HERE,

                                             REPEAT,
                                             UNTIL,

                                             LOOP,
                                             END_LOOP,

                                             IF,
                                             ELSE,
                                             END_IF,

                                             VIEW_DEFINITIONS,
                                             VIEW_OBJECTS,
                                             HELP
                                            };
  }
}