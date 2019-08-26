using System;
using System.Collections.Generic;
using System.Linq;
using Forth.ForthObjects;
using Forth.ForthObjects.Commands;

namespace Forth.Run
{
    public static class Parser
    {
        /// <summary>
        /// Main Parser function. Takes string of tokens and parses into Command elements.
        /// Most Commands will just be plain strings, but some ('if', 'loop' etc. will be
        /// dedicated commands with properties specifying the sub-commands to be executed)
        /// </summary>
        /// <param name="tokens">Array of tokens to parse</param>
        /// <param name="tokenIndex">Initial token index</param>
        /// <param name="stopTokens">Array of stop-tokens. E.g. 'If' parsing should stop on 'else' or 'endif'</param>
        /// <returns>Array of Command objects</returns>
        public static Command[] Parse(string[] tokens, ref int tokenIndex, string[] stopTokens)
        {
            List<Command> parsedTokens = new List<Command>();

            while (tokenIndex < tokens.Length)
            {
                string token = tokens[tokenIndex];
                if (token.Equals(Constants.If))
                {
                    IfCommand ifCommand = new IfCommand();

                    tokenIndex++;
                    ifCommand.IfCommands.AddRange(Parse(tokens, ref tokenIndex,
                        new[] {Constants.Else, Constants.EndIf}));
                    // Only parse the following tokens in our 'if' statement if the next token after the 'then' phase is *not*
                    // 'endif'. This is to handle 'else-less' 'if's where we are only programmed to handle what happens when 
                    // the 'if' evaluation is true, and to do nothing when the 'if' evaluation is false.
                    if (tokenIndex >= 1 && !tokens[tokenIndex - 1].Equals(Constants.EndIf))
                    {
                        ifCommand.ElseCommands.AddRange(Parse(tokens, ref tokenIndex, new[] {Constants.EndIf}));
                    }

                    parsedTokens.Add(ifCommand);
                }
                else if (token.Equals(Constants.Loop))
                {
                    LoopCommand loopCommand = new LoopCommand();

                    tokenIndex++;
                    loopCommand.LoopCommands.AddRange(Parse(tokens, ref tokenIndex, new[] {Constants.EndLoop}));

                    parsedTokens.Add(loopCommand);
                }
                else if (token.Equals(Constants.Repeat))
                {
                    RepeatCommand repeatCommand = new RepeatCommand();

                    tokenIndex++;
                    repeatCommand.RepeatCommands.AddRange(Parse(tokens, ref tokenIndex,
                        new[] {Constants.Until}));

                    parsedTokens.Add(repeatCommand);
                }
                else
                {
                    // For anything that's not an 'if', or loop, just add the string as a command.
                    // The executor can handle if it's not an actual command
                    parsedTokens.Add(new Command(token));

                    if ((stopTokens != null) && (stopTokens.Contains(token)))
                    {
                        tokenIndex++;
                        return parsedTokens.ToArray();
                    }

                    tokenIndex++;
                }
            }

            if (stopTokens != null)
            {
                // If we get here then 'stopTokens' was set to something that should terminate a sequence of commands,
                // but wasn't found. E.G. An 'if' without an 'endif'.
                throw new Exception(string.Format(Resources.Expected_, string.Join("/", stopTokens)));
            }

            return parsedTokens.ToArray();
        }
    }
}