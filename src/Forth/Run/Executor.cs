using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Forth.ForthObjects;
using Forth.ForthObjects.Commands;

namespace Forth.Run
{
    public static class Executor
    {
        /// <summary>
        /// Enum for IO mode (whether we display values as decimal, hex, double or char
        /// </summary>
        private enum IoModeEnum
        {
            Decimal,
            Fraction,
            Hex,
            Char
        };

        /// <summary>
        /// Current IO mode. Default to decimal.
        /// </summary>
        private static IoModeEnum _ioMode = IoModeEnum.Decimal;

        /// <summary>
        /// Enum for objects held in memory
        /// </summary>
        private enum MemoryEntryEnum
        {
            Variable,
            Value,
            Constant
        };

        /// <summary>
        /// The actual Forth Stack
        /// </summary>
        private static readonly FStack Stack = new FStack();

        /// <summary>
        /// Dictionary of Definitions (functions to you or me)
        /// Each key is the name and each value is the list of commands for the definition
        /// </summary>
        private static readonly Dictionary<string, List<Command>> Definitions = new Dictionary<string, List<Command>>();

        /// <summary>
        /// Dictionary of Objects (Variables, Values, Constants (and Arrays))
        /// The key is the object name and each value is a tuple consisting of the type of the object
        /// (variable, constant etc.) and a pointer to the memory location
        /// </summary>
        private static readonly Dictionary<string, Tuple<MemoryEntryEnum, int>> Objects =
            new Dictionary<string, Tuple<MemoryEntryEnum, int>>();

        /// <summary>
        /// Byte array for our memory. All variables, values, constants and arrays will be allocated here.
        /// </summary>
        private static readonly byte[] Memory = new byte[Constants.MemorySize];

        /// <summary>
        /// The current memory pointer. Defaulting to the start of our Memory array, and incrementing
        /// each time we declare an object.
        /// </summary>
        private static int _memoryPointer;


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
            if (depth > Constants.MaxProcessingStack)
            {
                throw new Exception(Resources.ProcessingStackOverflow);
            }

            if (depth < 0) throw new ArgumentOutOfRangeException(nameof(depth));

            while (currentCommandIndex < commands.Length)
            {
                Command currentCommand = commands[currentCommandIndex];

                //Console.WriteLine(currentCommand.Name);

                if (currentCommand.Name.Equals(Constants.StartDefinition))
                {
                    currentCommandIndex++;
                    ParseDefinition(commands, ref currentCommandIndex, false);
                }
                else if (currentCommand.Name.Equals(Constants.RedefineDefinition))
                {
                    currentCommandIndex++;
                    ParseDefinition(commands, ref currentCommandIndex, true);
                }
                else if (currentCommand is IfCommand)
                {
                    IfCommand ifCommand = currentCommand as IfCommand;
                    FInteger intValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
                    if (intValue.Value == Constants.True)
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
                else if (currentCommand.Name.Equals(Constants.Else))
                {
                }
                else if (currentCommand.Name.Equals(Constants.EndIf))
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
                    } while (fInteger.Value == Constants.False);
                }
                else if (currentCommand.Name.Equals(Constants.Until))
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
                else if (currentCommand.Name.Equals(Constants.EndLoop))
                {
                }
                else
                {
                    if ((_ioMode == IoModeEnum.Hex) && (IsHexInteger(currentCommand.Name)))
                    {
                        int intValue;

                        if (!int.TryParse(currentCommand.Name, System.Globalization.NumberStyles.HexNumber, null,
                            out intValue))
                        {
                            throw new Exception(string.Format(Resources.InvalidHexInteger, currentCommand));
                        }

                        FInteger intConstant = new FInteger(intValue);
                        Stack.Push(new Cell(intConstant.GetBytes()));
                    }
                    else if ((_ioMode == IoModeEnum.Decimal) && (IsInteger(currentCommand.Name)))
                    {
                        int intValue;
                        if (!int.TryParse(currentCommand.Name, out intValue))
                        {
                            throw new Exception(string.Format(Resources.InvalidInteger, currentCommand));
                        }

                        FInteger intConstant = new FInteger(intValue);
                        Stack.Push(new Cell(intConstant.GetBytes()));
                    }
                    else
                    {
                        float floatValue;
                        if ((_ioMode == IoModeEnum.Fraction) && (IsFloat(currentCommand.Name)))
                        {
                            if (!float.TryParse(currentCommand.Name, out floatValue))
                            {
                                throw new Exception(string.Format(Resources.InvalidFloat, currentCommand));
                            }

                            FFloat intConstant = new FFloat(floatValue);
                            Stack.Push(new Cell(intConstant.GetBytes()));
                        }
                        else if ((_ioMode == IoModeEnum.Char) && (IsChar(currentCommand.Name)))
                        {
                            string subString = currentCommand.Name.Substring(Constants.StartChar.Length,
                                currentCommand.Name.Length - Constants.EndChar.Length - 1);
                            char ch;
                            if (!char.TryParse(subString, out ch))
                            {
                                throw new Exception(string.Format(Resources.InvalidCharacter, subString));
                            }

                            FInteger intConstant = new FInteger(ch);
                            Stack.Push(new Cell(intConstant.GetBytes()));
                        }
                        else if (IsString(currentCommand.Name))
                        {
                            string subString = currentCommand.Name.Substring(Constants.StartString.Length,
                                currentCommand.Name.Length - Constants.EndString.Length - 2);
                            Console.Write(subString);
                        }
                        else if (IsVariable(currentCommand.Name))
                        {
                            Stack.Push(new Cell(new FInteger(Objects[currentCommand.Name].Item2).GetBytes()));
                        }
                        else if (IsValue(currentCommand.Name))
                        {
                            byte[] bytes = new byte[Constants.CellSize];
                            Array.Copy(Memory, Objects[currentCommand.Name].Item2, bytes, 0, Constants.CellSize);
                            Stack.Push(new Cell(bytes));
                        }
                        else if (IsConstant(currentCommand.Name))
                        {
                            byte[] bytes = new byte[Constants.CellSize];
                            Array.Copy(Memory, Objects[currentCommand.Name].Item2, bytes, 0, Constants.CellSize);
                            Stack.Push(new Cell(bytes));
                        }
                        else if (IsDefinition(currentCommand.Name))
                        {
                            int definitionTokenIndex = 0;
                            ProcessCommands(Definitions[currentCommand.Name].ToArray(), ref definitionTokenIndex, depth + 1);
                        }
                        else if (currentCommand.Name.Equals(Constants.ViewDefinitions))
                        {
                            Console.WriteLine();
                            foreach (string key in Definitions.Keys)
                            {
                                Console.Write(Resources.Executor_ProcessCommands__StartDefinition, Constants.StartDefinition);
                                Console.Write(string.Format(Resources.Executor_ProcessCommands__Key, key).PadRight(18, ' '));
                                foreach (Command command in Definitions[key])
                                {
                                    Console.Write(string.Format("{0} ", command.Name));
                                }

                                Console.WriteLine(string.Format("{0}", Constants.EndDefinition));
                            }
                        }
                        else if (currentCommand.Name.Equals(Constants.ViewObjects))
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
                        else if (currentCommand.Name.Equals(Constants.Help))
                        {
                            Console.WriteLine();
                            foreach (string command in Constants.ValidCommands)
                            {
                                Console.WriteLine(command);
                            }
                        }
                        else if (currentCommand.Name.Equals(Constants.Variable))
                        {
                            if (currentCommandIndex >= commands.Length - 1)
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
                        else if (currentCommand.Name.Equals(Constants.Value))
                        {
                            if (currentCommandIndex >= commands.Length - 1)
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
                        else if (currentCommand.Name.Equals(Constants.Constant))
                        {
                            if (currentCommandIndex >= commands.Length - 1)
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
                        else if (currentCommand.Name.Equals(Constants.Store))
                        {
                            FInteger memoryLocation = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
                            FInteger intValue = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));

                            StoreToMemory(intValue, memoryLocation);
                        }
                        else if (currentCommand.Name.Equals(Constants.Fetch))
                        {
                            FInteger memoryLocation = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));

                            byte[] bytes = FetchFromMemory(memoryLocation);

                            FInteger intValue = new FInteger(BitConverter.ToInt32(bytes, 0));
                            Stack.Push(new Cell(intValue.GetBytes()));
                        }
                        else if (currentCommand.Name.Equals(Constants.To))
                        {
                            if (currentCommandIndex >= commands.Length - 1)
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
                        else if (currentCommand.Name.Equals(Constants.Cell))
                        {
                            Stack.Push(new Cell(new FInteger(Constants.CellSize).GetBytes()));
                        }
                        else if (currentCommand.Name.Equals(Constants.Here))
                        {
                            Stack.Push(new Cell(new FInteger(_memoryPointer).GetBytes()));
                        }
                        else if (currentCommand.Name.Equals(Constants.Allot))
                        {
                            FInteger memoryAllocBytes = new FInteger(BitConverter.ToInt32(Stack.Pop().Bytes, 0));
                            if (_memoryPointer + memoryAllocBytes.Value < 0)
                            {
                                throw new Exception(Resources.MemoryUnderflow);
                            }
                            else if (_memoryPointer + memoryAllocBytes.Value > Constants.MemorySize)
                            {
                                throw new Exception(Resources.MemoryOverflow);
                            }

                            _memoryPointer += memoryAllocBytes.Value;
                        }
                        else if (currentCommand.Name.Equals(Constants.Decimal))
                        {
                            _ioMode = IoModeEnum.Decimal;
                        }
                        else if (currentCommand.Name.Equals(Constants.Hex))
                        {
                            _ioMode = IoModeEnum.Hex;
                        }
                        else if (currentCommand.Name.Equals(Constants.Fractional))
                        {
                            _ioMode = IoModeEnum.Fraction;
                        }
                        else if (currentCommand.Name.Equals(Constants.Char))
                        {
                            _ioMode = IoModeEnum.Char;
                        }
                        else if (currentCommand.Name.Equals(Constants.Period))
                        {
                            OutputValue(Stack.Pop().Bytes);
                        }
                        else if (currentCommand.Name.Equals(Constants.Dup))
                        {
                            Stack.Dup();
                        }
                        else if (currentCommand.Name.Equals(Constants.Swap))
                        {
                            Stack.Swap();
                        }
                        else if (currentCommand.Name.Equals(Constants.Drop))
                        {
                            Stack.Drop();
                        }
                        else if (currentCommand.Name.Equals(Constants.Rot))
                        {
                            Stack.Rot();
                        }
                        else if (currentCommand.Name.Equals(Constants.Over))
                        {
                            Stack.Over();
                        }
                        else if (currentCommand.Name.Equals(Constants.Tuck))
                        {
                            Stack.Tuck();
                        }
                        else if (currentCommand.Name.Equals(Constants.Roll))
                        {
                            Stack.Roll();
                        }
                        else if (currentCommand.Name.Equals(Constants.Pick))
                        {
                            Stack.Pick();
                        }
                        else if (currentCommand.Name.Equals(Constants.Cr))
                        {
                            Console.WriteLine();
                        }
                        else if (
                            currentCommand.Name.Equals(Constants.Add) ||
                            currentCommand.Name.Equals(Constants.Subtract) ||
                            currentCommand.Name.Equals(Constants.Multiply) ||
                            currentCommand.Name.Equals(Constants.Modulus) ||
                            currentCommand.Name.Equals(Constants.Divide) ||
                            currentCommand.Name.Equals(Constants.GreaterThan) ||
                            currentCommand.Name.Equals(Constants.LessThan) ||
                            currentCommand.Name.Equals(Constants.GreaterThanOrEqual) ||
                            currentCommand.Name.Equals(Constants.LessThanOrEqual) ||
                            currentCommand.Name.Equals(Constants.Equal) ||
                            currentCommand.Name.Equals(Constants.NotEqual) ||
                            currentCommand.Name.Equals(Constants.And) ||
                            currentCommand.Name.Equals(Constants.Or) ||
                            currentCommand.Name.Equals(Constants.Not)
                        )
                        {
                            if (_ioMode == IoModeEnum.Fraction)
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
                    }
                }

                // Increment to next token
                currentCommandIndex++;
            }
        }

        private static void ParseDefinition(Command[] commands, ref int currentCommandIndex, bool redefine)
        {
            if (commands.Length < currentCommandIndex)
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

            if (currentCommandIndex >= commands.Length)
            {
                throw new Exception(Resources.ExpectedEndDefinition);
            }

            Definitions.Add(definitionName, new List<Command>());

            while (!commands[currentCommandIndex].Name.Equals(Constants.EndDefinition))
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

                if (currentCommandIndex >= commands.Length)
                {
                    throw new Exception(Resources.ExpectedEndDefinition);
                }
            }
        }

        #region Command checking (IsCommand? IsDefinition? IsInteger? etc.)

        private static bool IsCommand(string token)
        {
            return Constants.ValidCommands.Contains(token);
        }

        private static bool IsDefinition(string token)
        {
            return Definitions.ContainsKey(token);
        }

        private static bool IsHexInteger(string token)
        {
            int someInt;
            return int.TryParse(token, NumberStyles.HexNumber, null, out someInt);
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
            return token.StartsWith(Constants.StartChar) && token.EndsWith(Constants.EndChar);
        }

        private static bool IsString(string token)
        {
            return token.StartsWith(Constants.StartString) && token.EndsWith(Constants.EndString);
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
            if (_ioMode == IoModeEnum.Decimal)
            {
                FInteger intObject = new FInteger(BitConverter.ToInt32(bytes, 0));
                return intObject.Value.ToString();
            }
            else if (_ioMode == IoModeEnum.Fraction)
            {
                FFloat floatObject = new FFloat(BitConverter.ToSingle(bytes, 0));
                return floatObject.Value.ToString(CultureInfo.InvariantCulture);
            }
            else if (_ioMode == IoModeEnum.Hex)
            {
                FInteger intObject = new FInteger(BitConverter.ToInt32(bytes, 0));
                return intObject.Value.ToString("X2");
            }
            else if (_ioMode == IoModeEnum.Char)
            {
                FInteger intObject = new FInteger(BitConverter.ToInt32(bytes, 0));
                return ((char) intObject.Value).ToString();
            }

            return string.Empty;
        }

        private static void OutputValue(byte[] bytes)
        {
            Console.Write(GetOutputValue(bytes) + Resources.Executor_OutputValue_Space);
        }

        public static void OutputError(string message)
        {
            Console.WriteLine(Resources.Executor_OutputError_ERROR___0_, message);
        }

        #endregion

        #region Variables and Memory

        private static void StoreToMemory(FInteger intValue, FInteger memoryLocation)
        {
            if (memoryLocation.Value < 0)
            {
                throw new Exception(Resources.MemoryUnderflow);
            }
            else if (memoryLocation.Value + Constants.CellSize > Constants.MemorySize)
            {
                throw new Exception(Resources.MemoryOverflow);
            }

            Array.Copy(intValue.GetBytes(), 0, Memory, memoryLocation.Value, Constants.CellSize);
        }

        private static byte[] FetchFromMemory(FInteger memoryLocation)
        {
            byte[] bytes = new byte[Constants.CellSize];
            Array.Copy(Memory, memoryLocation.Value, bytes, 0, Constants.CellSize);

            return bytes;
        }

        private static void AddObject(string name, MemoryEntryEnum memoryEntry)
        {
            AddObject(name, memoryEntry, new byte[Constants.CellSize]);
        }

        private static void AddObject(string name, MemoryEntryEnum memoryEntry, byte[] bytes)
        {
            if (_memoryPointer + Constants.CellSize >= Constants.MemorySize)
            {
                throw new Exception(Resources.MemoryOverflow);
            }

            Objects.Add(name, new Tuple<MemoryEntryEnum, int>(memoryEntry, _memoryPointer));

            SetObject(_memoryPointer, bytes);

            _memoryPointer += Constants.CellSize;
        }

        private static void SetObject(int location, byte[] bytes)
        {
            Array.Copy(bytes, 0, Memory, location, Constants.CellSize);
        }

        #endregion
    }
}