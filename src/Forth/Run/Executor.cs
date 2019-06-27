using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forth
{
  public static class Executor
  {
    /// <summary>
    /// Enum for IO mode (whether we display values as decimal, hex, double or char
    /// </summary>
    private enum IOModeEnum { Decimal, Fraction, Hex, Char };

    /// <summary>
    /// Current IO mode. Default to decimal.
    /// </summary>
    private static IOModeEnum IOMode = IOModeEnum.Decimal;

    /// <summary>
    /// Enum for objects held in memory
    /// </summary>
    private enum MemoryEntryEnum { Variable, Value, Constant };

    /// <summary>
    /// The actual Forth Stack
    /// </summary>
    private static FStack Stack = new FStack();

    /// <summary>
    /// Dictionary of Definitions (functions to you or me)
    /// Each key is the name and each value is the list of commands for the definition
    /// </summary>
    private static Dictionary<string, List<Command>> Definitions = new Dictionary<string, List<Command>>();

    /// <summary>
    /// Dictionary of Objects (Variables, Values, Constants (and Arrays))
    /// The key is the object name and each value is a tuple consisting of the type of the object
    /// (variable, constant etc.) and a pointer to the memory location
    /// </summary>
    private static Dictionary<string, Tuple<MemoryEntryEnum, int>> Objects = new Dictionary<string, Tuple<MemoryEntryEnum, int>>();

    /// <summary>
    /// Byte array for our memory. All variables, values, constants and arrays will be allocated here.
    /// </summary>
    private static byte[] Memory = new byte[Constants.MEMORY_SIZE];

    /// <summary>
    /// The current memory pointer. Defaulting to the start of our Memory array, and incrementing
    /// each time we declare an object.
    /// </summary>
    private static int MemoryPointer = 0;


    public static bool ProcessString(string command)
    {
      try
      {
        string[] tokens = Tokeniser.Tokenise(command);

        int tokenIndex = 0;
        Command[] commandTokens = Parser.Parse(tokens, ref tokenIndex, null);

        int commandIndex = 0;
        ProcessCommands(commandTokens, ref commandIndex, 0);
        return true;
      }
      catch (Exception ex)
      {
        OutputError(ex.Message);
        return false;
      }
    }

    private static void ProcessCommands(Command[] commands, ref int currentCommandIndex, int depth)
    {
      if (depth > Constants.MAX_PROCESSING_STACK)
      {
        throw new Exception(Resources.ProcessingStackOverflow);
      }

      while (currentCommandIndex < commands.Count())
      {
        Command currentCommand = commands[currentCommandIndex];

        //Console.WriteLine(currentCommand.Name);

        if (currentCommand.Name.Equals(Constants.START_DEFINITION))
        {
          currentCommandIndex++;
          ParseDefinition(commands, ref currentCommandIndex, false);
        }
        else if (currentCommand.Name.Equals(Constants.REDEFINE_DEFINITION))
        {
          currentCommandIndex++;
          ParseDefinition(commands, ref currentCommandIndex, true);
        }
        else if (currentCommand is IfCommand)
        {
          IfCommand ifCommand = currentCommand as IfCommand;
          FInteger intValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
          if (intValue.Value == Constants.TRUE)
          {
            int ifCommandIndex = 0;
            ProcessCommands(ifCommand.IfCommands.ToArray(), ref ifCommandIndex, depth + 1);
          }
          else
          {
            int elseCommandIndex = 0;
            ProcessCommands(ifCommand.ElseCommands.ToArray(), ref elseCommandIndex, depth + 1);
          }
        }
        else if (currentCommand.Name.Equals(Constants.ELSE))
        {
        }
        else if (currentCommand.Name.Equals(Constants.END_IF))
        {
        }
        else if (currentCommand is RepeatCommand)
        {
          RepeatCommand repeatCommand = currentCommand as RepeatCommand;
          FInteger fInteger;
          do
          {
            int repeatCommandIndex = 0;
            ProcessCommands(repeatCommand.RepeatCommands.ToArray(), ref repeatCommandIndex, depth + 1);
            fInteger = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
          } while (fInteger.Value == Constants.FALSE);
        }
        else if (currentCommand.Name.Equals(Constants.UNTIL))
        {
        }
        else if (currentCommand is LoopCommand)
        {
          LoopCommand loopCommand = currentCommand as LoopCommand;

          FInteger incrementor = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
          FInteger currentValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
          FInteger targetValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));

          while (currentValue.Value != targetValue.Value)
          {
            Stack.Push(new Cell(targetValue.GetBytes()));
            Stack.Push(new Cell(currentValue.GetBytes()));
            Stack.Push(new Cell(incrementor.GetBytes()));
            int loopCommandIndex = 0;
            ProcessCommands(loopCommand.LoopCommands.ToArray(), ref loopCommandIndex, depth + 1);

            incrementor = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
            currentValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
            targetValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
            currentValue.Value += incrementor.Value;
          }
        }
        else if (currentCommand.Name.Equals(Constants.END_LOOP))
        {
        }
        else if ((IOMode == IOModeEnum.Hex) && (IsHexInteger(currentCommand.Name)))
        {
          Int32 intValue;
          if (!int.TryParse(currentCommand.Name, System.Globalization.NumberStyles.HexNumber, null, out intValue))
          {
            throw new Exception(string.Format(Resources.InvalidHexInteger, currentCommand));
          }
          FInteger intConstant = new FInteger(intValue);
          Stack.Push(new Cell(intConstant.GetBytes()));
        }
        else if ((IOMode == IOModeEnum.Decimal) && (IsInteger(currentCommand.Name)))
        {
          Int32 intValue;
          if (!Int32.TryParse(currentCommand.Name, out intValue))
          {
            throw new Exception(string.Format(Resources.InvalidInteger, currentCommand));
          }
          FInteger intConstant = new FInteger(intValue);
          Stack.Push(new Cell(intConstant.GetBytes()));
        }
        else if ((IOMode == IOModeEnum.Fraction) && (IsFloat(currentCommand.Name)))
        {
          float floatValue;
          if (!float.TryParse(currentCommand.Name, out floatValue))
          {
            throw new Exception(string.Format(Resources.InvalidFloat, currentCommand));
          }
          FFloat intConstant = new FFloat(floatValue);
          Stack.Push(new Cell(intConstant.GetBytes()));
        }
        else if ((IOMode == IOModeEnum.Char) && (IsChar(currentCommand.Name)))
        {
          string subString = currentCommand.Name.Substring(Constants.START_CHAR.Length, currentCommand.Name.Length - Constants.END_CHAR.Length - 1);
          char ch;
          if (!char.TryParse(subString, out ch))
          {
            throw new Exception(string.Format(Resources.InvalidCharacter, subString));
          }

          FInteger intConstant = new FInteger((Int32)ch);
          Stack.Push(new Cell(intConstant.GetBytes()));
        }
        else if (IsString(currentCommand.Name))
        {
          string subString = currentCommand.Name.Substring(Constants.START_STRING.Length, currentCommand.Name.Length - Constants.END_STRING.Length - 2);
          Console.Write(subString);
        }
        else if (IsVariable(currentCommand.Name))
        {
          Stack.Push(new Cell(new FInteger(Objects[currentCommand.Name].Item2).GetBytes()));
        }
        else if (IsValue(currentCommand.Name))
        {
          byte[] bytes = new byte[Constants.CELL_SIZE];
          Array.Copy(Memory, Objects[currentCommand.Name].Item2, bytes, 0, Constants.CELL_SIZE);
          Stack.Push(new Cell(bytes));
        }
        else if (IsConstant(currentCommand.Name))
        {
          byte[] bytes = new byte[Constants.CELL_SIZE];
          Array.Copy(Memory, Objects[currentCommand.Name].Item2, bytes, 0, Constants.CELL_SIZE);
          Stack.Push(new Cell(bytes));
        }
        else if (IsDefinition(currentCommand.Name))
        {
          int definitionTokenIndex = 0;
          ProcessCommands(Definitions[currentCommand.Name].ToArray(), ref definitionTokenIndex, depth + 1);
        }
        else if (currentCommand.Name.Equals(Constants.VIEW_DEFINITIONS))
        {
          Console.WriteLine();
          foreach (string key in Definitions.Keys)
          {
            Console.Write(string.Format("{0} ", Constants.START_DEFINITION));
            Console.Write(string.Format("{0}", key).PadRight(18, ' '));
            foreach (Command command in Definitions[key])
            {
              Console.Write(string.Format("{0} ", command.Name));
            }
            Console.WriteLine(string.Format("{0}", Constants.END_DEFINITION));
          }
        }
        else if (currentCommand.Name.Equals(Constants.VIEW_OBJECTS))
        {
          Console.WriteLine();
          foreach (string key in Objects.Keys)
          {
            Console.Write(string.Format("{0}", key).PadRight(20, ' '));
            Console.Write(string.Format("{0}\t", Objects[key].Item1));
            Console.Write(string.Format("{0}\t", Objects[key].Item2));

            byte[] bytes = FetchFromMemory(new FInteger(Objects[key].Item2));
            Console.WriteLine(string.Format("{0}", GetOutputValue(bytes)));
          }
        }
        else if (currentCommand.Name.Equals(Constants.HELP))
        {
          Console.WriteLine();
          foreach (string command in Constants.VALID_COMMANDS)
          {
            Console.WriteLine(command);
          }
        }
        else if (currentCommand.Name.Equals(Constants.VARIABLE))
        {
          if (currentCommandIndex >= commands.Count() - 1)
          {
            throw new Exception(Resources.ExpectedAName);
          }
          currentCommandIndex++;
          string variableName = commands[currentCommandIndex].Name;
          if (IsAlreadyDefined(variableName))
          {
            throw new Exception(string.Format(Resources.AlreadyDefined, variableName));
          }

          AddObject(variableName, MemoryEntryEnum.Variable);
        }
        else if (currentCommand.Name.Equals(Constants.VALUE))
        {
          if (currentCommandIndex >= commands.Count() - 1)
          {
            throw new Exception(Resources.ExpectedAName);
          }
          currentCommandIndex++;
          string variableName = commands[currentCommandIndex].Name;
          if (IsAlreadyDefined(variableName))
          {
            throw new Exception(string.Format(Resources.AlreadyDefined, variableName));
          }

          AddObject(variableName, MemoryEntryEnum.Value, Stack.Pop().Bytes);
        }
        else if (currentCommand.Name.Equals(Constants.CONSTANT))
        {
          if (currentCommandIndex >= commands.Count() - 1)
          {
            throw new Exception(Resources.ExpectedAName);
          }
          currentCommandIndex++;
          string variableName = commands[currentCommandIndex].Name;
          if (IsAlreadyDefined(variableName))
          {
            throw new Exception(string.Format(Resources.AlreadyDefined, variableName));
          }

          AddObject(variableName, MemoryEntryEnum.Constant, Stack.Pop().Bytes);
        }
        else if (currentCommand.Name.Equals(Constants.STORE))
        {
          FInteger memoryLocation = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
          FInteger intValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));

          StoreToMemory(intValue, memoryLocation);
        }
        else if (currentCommand.Name.Equals(Constants.FETCH))
        {
          FInteger memoryLocation = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));

          byte[] bytes = FetchFromMemory(memoryLocation);

          FInteger intValue = new FInteger(BitConverter.ToInt32(bytes, 0));
          Stack.Push(new Cell(intValue.GetBytes()));
        }
        else if (currentCommand.Name.Equals(Constants.TO))
        {
          if (currentCommandIndex >= commands.Count() - 1)
          {
            throw new Exception(Resources.ExpectedAName);
          }
          currentCommandIndex++;
          string valueName = commands[currentCommandIndex].Name;
          if (!IsValue(valueName))
          {
            throw new Exception(string.Format(Resources.NameIsNotAValue, valueName));
          }

          SetObject(Objects[valueName].Item2, Stack.Pop().Bytes);
        }
        else if (currentCommand.Name.Equals(Constants.CELL))
        {
          Stack.Push(new Cell(new FInteger(Constants.CELL_SIZE).GetBytes()));
        }
        else if (currentCommand.Name.Equals(Constants.HERE))
        {
          Stack.Push(new Cell(new FInteger(MemoryPointer).GetBytes()));
        }
        else if (currentCommand.Name.Equals(Constants.ALLOT))
        {
          FInteger memoryAllocBytes = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
          if (MemoryPointer + memoryAllocBytes.Value < 0)
          {
            throw new Exception(Resources.MemoryUnderflow);
          }
          else if (MemoryPointer + memoryAllocBytes.Value > Constants.MEMORY_SIZE)
          {
            throw new Exception(Resources.MemoryOverflow);
          }

          MemoryPointer += memoryAllocBytes.Value;
        }
        else if (currentCommand.Name.Equals(Constants.DECIMAL))
        {
          IOMode = IOModeEnum.Decimal;
        }
        else if (currentCommand.Name.Equals(Constants.HEX))
        {
          IOMode = IOModeEnum.Hex;
        }
        else if (currentCommand.Name.Equals(Constants.FRACTIONAL))
        {
          IOMode = IOModeEnum.Fraction;
        }
        else if (currentCommand.Name.Equals(Constants.CHAR))
        {
          IOMode = IOModeEnum.Char;
        }
        else if (currentCommand.Name.Equals(Constants.PERIOD))
        {
          OutputValue(Stack.Pop().Bytes);
        }
        else if (currentCommand.Name.Equals(Constants.DUP))
        {
          Stack.Dup();
        }
        else if (currentCommand.Name.Equals(Constants.SWAP))
        {
          Stack.Swap();
        }
        else if (currentCommand.Name.Equals(Constants.DROP))
        {
          Stack.Drop();
        }
        else if (currentCommand.Name.Equals(Constants.ROT))
        {
          Stack.Rot();
        }
        else if (currentCommand.Name.Equals(Constants.OVER))
        {
          Stack.Over();
        }
        else if (currentCommand.Name.Equals(Constants.TUCK))
        {
          Stack.Tuck();
        }
        else if (currentCommand.Name.Equals(Constants.ROLL))
        {
          Stack.Roll();
        }
        else if (currentCommand.Name.Equals(Constants.PICK))
        {
          Stack.Pick();
        }
        else if (currentCommand.Name.Equals(Constants.CR))
        {
          Console.WriteLine();
        }
        else if (
          currentCommand.Name.Equals(Constants.ADD) ||
          currentCommand.Name.Equals(Constants.SUBTRACT) ||
          currentCommand.Name.Equals(Constants.MULTIPLY) ||
          currentCommand.Name.Equals(Constants.MODULUS) ||
          currentCommand.Name.Equals(Constants.DIVIDE) ||
          currentCommand.Name.Equals(Constants.GREATER_THAN) ||
          currentCommand.Name.Equals(Constants.LESS_THAN) ||
          currentCommand.Name.Equals(Constants.GREATER_THAN_OR_EQUAL) ||
          currentCommand.Name.Equals(Constants.LESS_THAN_OR_EQUAL) ||
          currentCommand.Name.Equals(Constants.EQUAL) ||
          currentCommand.Name.Equals(Constants.NOT_EQUAL) ||
          currentCommand.Name.Equals(Constants.AND) ||
          currentCommand.Name.Equals(Constants.OR) ||
          currentCommand.Name.Equals(Constants.NOT)
          )
        {
          if (IOMode == IOModeEnum.Fraction)
          {
            Stack.FloatMaths(currentCommand.Name);
          }
          else
          {
            Stack.IntMaths(currentCommand.Name);
          }
        }
        else
        {
          throw new Exception(string.Format(Resources.UnknownItem, currentCommand.Name));
        }

        // Increment to next token
        currentCommandIndex++;
      }
    }

    private static void ParseDefinition(Command[] commands, ref int currentCommandIndex, bool redefine)
    {
      if (commands.Count() < currentCommandIndex)
      {
        throw new Exception(Resources.ExpectedAName);
      }

      string definitionName = commands[currentCommandIndex].Name;
      currentCommandIndex++;

      if (IsAlreadyDefined(definitionName))
      {
        if (redefine)
        {
          Definitions.Remove(definitionName);
        }
        else
        {
          throw new Exception(string.Format(Resources.AlreadyDefined, definitionName));
        }
      }

      if (currentCommandIndex >= commands.Count())
      {
        throw new Exception(Resources.ExpectedEndDefinition);
      }

      Definitions.Add(definitionName, new List<Command>());

      while (!commands[currentCommandIndex].Name.Equals(Constants.END_DEFINITION))
      {
        string commandName = commands[currentCommandIndex].Name;
        if (
          !IsCommand(commandName) &&
          !IsValue(commandName) &&
          !IsVariable(commandName) &&
          !IsConstant(commandName) &&
          !IsInteger(commandName) &&
          !IsChar(commandName) &&
          !IsString(commandName) &&
          !IsDefinition(commandName)
          )
        {
          throw new Exception(string.Format(Resources.UnknownItem, commandName));
        }

        Definitions[definitionName].Add(commands[currentCommandIndex]);

        currentCommandIndex++;

        if (currentCommandIndex >= commands.Count())
        {
          throw new Exception(Resources.ExpectedEndDefinition);
        }
      }
    }

    #region Command checking (IsCommand? IsDefinition? IsInteger? etc.)

    private static bool IsCommand(string token)
    {
      return Constants.VALID_COMMANDS.Contains(token);
    }

    private static bool IsDefinition(string token)
    {
      return Definitions.ContainsKey(token);
    }

    private static bool IsHexInteger(string token)
    {
      int someInt;
      return int.TryParse(token, System.Globalization.NumberStyles.HexNumber, null, out someInt);
    }

    private static bool IsInteger(string token)
    {
      int someInt;
      return int.TryParse(token, out someInt);
    }

    private static bool IsFloat(string token)
    {
      float someFloat;
      return float.TryParse(token, out someFloat);
    }

    private static bool IsChar(string token)
    {
      return token.StartsWith(Constants.START_CHAR) && token.EndsWith(Constants.END_CHAR);
    }

    private static bool IsString(string token)
    {
      return token.StartsWith(Constants.START_STRING) && token.EndsWith(Constants.END_STRING);
    }

    private static bool IsVariable(string name)
    {
      return Objects.ContainsKey(name) && Objects[name].Item1 == MemoryEntryEnum.Variable;
    }

    private static bool IsValue(string name)
    {
      return Objects.ContainsKey(name) && Objects[name].Item1 == MemoryEntryEnum.Value;
    }

    private static bool IsConstant(string name)
    {
      return Objects.ContainsKey(name) && Objects[name].Item1 == MemoryEntryEnum.Constant;
    }

    private static bool IsAlreadyDefined(string name)
    {
      return (
        IsCommand(name) ||
        IsDefinition(name) ||
        IsVariable(name) ||
        IsValue(name) ||
        IsConstant(name));
    }

    #endregion

    #region Output

    private static string GetOutputValue(byte[] bytes)
    {
      if (IOMode == IOModeEnum.Decimal)
      {
        FInteger intObject = new FInteger(BitConverter.ToInt32(bytes, 0));
        return intObject.Value.ToString();
      }
      else if (IOMode == IOModeEnum.Fraction)
      {
        FFloat floatObject = new FFloat(BitConverter.ToSingle(bytes, 0));
        return floatObject.Value.ToString();
      }
      else if (IOMode == IOModeEnum.Hex)
      {
        FInteger intObject = new FInteger(BitConverter.ToInt32(bytes, 0));
        return intObject.Value.ToString("X2");
      }
      else if (IOMode == IOModeEnum.Char)
      {
        FInteger intObject = new FInteger(BitConverter.ToInt32(bytes, 0));
        return ((char)intObject.Value).ToString();
      }

      return string.Empty;
    }

    private static void OutputValue(byte[] bytes)
    {
      Console.Write(GetOutputValue(bytes) + " ");
    }

    public static void OutputError(string message)
    {
      Console.WriteLine("ERROR: {0}", message);
    }

    #endregion

    #region Variables and Memory

    private static void StoreToMemory(FInteger intValue, FInteger memoryLocation)
    {
      if (memoryLocation.Value < 0)
      {
        throw new Exception(Resources.MemoryUnderflow);
      }
      else if (memoryLocation.Value + Constants.CELL_SIZE > Constants.MEMORY_SIZE)
      {
        throw new Exception(Resources.MemoryOverflow);
      }
      Array.Copy(intValue.GetBytes(), 0, Memory, memoryLocation.Value, Constants.CELL_SIZE);
    }

    private static byte[] FetchFromMemory(FInteger memoryLocation)
    {
      byte[] bytes = new byte[Constants.CELL_SIZE];
      Array.Copy(Memory, memoryLocation.Value, bytes, 0, Constants.CELL_SIZE);

      return bytes;
    }

    private static void AddObject(string name, MemoryEntryEnum memoryEntry)
    {
      AddObject(name, memoryEntry, new byte[Constants.CELL_SIZE]);
    }

    private static void AddObject(string name, MemoryEntryEnum memoryEntry, byte[] bytes)
    {
      if (MemoryPointer + Constants.CELL_SIZE >= Constants.MEMORY_SIZE)
      {
        throw new Exception(Resources.MemoryOverflow);
      }

      Objects.Add(name, new Tuple<MemoryEntryEnum, int>(memoryEntry, MemoryPointer));

      SetObject(MemoryPointer, bytes);

      MemoryPointer += Constants.CELL_SIZE;
    }

    private static void SetObject(int location, byte[] bytes)
    {
      Array.Copy(bytes, 0, Memory, location, Constants.CELL_SIZE);
    }

    #endregion
  }
}