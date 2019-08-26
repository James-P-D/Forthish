using System.Collections.Generic;
using Forth.ForthObjects;

namespace Forth.Run
{
    /// <summary>
    /// String Tokeniser class
    /// </summary>
    public static class Tokeniser
    {
        /// <summary>
        /// Main tokeniser method. Split input string on white-space (spaces
        /// tabs, carriage returns etc.
        /// </summary>
        /// <param name="input">Input string to split</param>
        /// <returns>Array of strings split on whitespace</returns>
        public static string[] Tokenise(string input)
        {
            List<string> tokens = new List<string>();

            string currentToken = string.Empty;
            bool inComment = false;
            bool inString = false;
            bool inChar = false;

            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];

                // Keep track of comments so we know when to keep tokens
                if (inComment)
                {
                    currentToken += ch;

                    if (ch.ToString().Equals(Constants.EndComment))
                    {
                        currentToken = string.Empty;
                        inComment = false;
                    }

                    continue;
                }

                // Keep track of strings so we don't split on whitespace that appears
                // mid-string. e.g. ."Hello World".
                if (inString)
                {
                    currentToken += ch;

                    if (ch.ToString().Equals(Constants.EndString) && currentToken.Length > 2)
                    {
                        currentToken = currentToken.Trim();
                        if (!string.IsNullOrEmpty(currentToken))
                        {
                            tokens.Add(currentToken);
                            currentToken = string.Empty;
                        }

                        inString = false;
                    }

                    continue;
                }

                if (inChar)
                {
                    currentToken += ch;

                    if (ch.ToString().Equals(Constants.EndChar))
                    {
                        currentToken = currentToken.Trim();
                        if (!string.IsNullOrEmpty(currentToken))
                        {
                            tokens.Add(currentToken);
                            currentToken = string.Empty;
                        }

                        inChar = false;
                    }

                    continue;
                }

                if (ch.Equals(' ') || ch.Equals('\t') || ch.Equals('\r') || ch.Equals('\n'))
                {
                    currentToken = currentToken.Trim();
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(currentToken);
                        currentToken = string.Empty;
                    }
                }
                else
                {
                    if (input.Substring(i, Constants.StartComment.Length).Equals(Constants.StartComment))
                    {
                        currentToken += ch;
                        inComment = true;
                    }
                    else if (input.Substring(i, Constants.StartChar.Length).Equals(Constants.StartComment))
                    {
                        currentToken += ch;
                        inChar = true;
                    }
                    else if (
                        (i < input.Length - Constants.StartString.Length) &&
                        (input.Substring(i, Constants.StartString.Length).Equals(Constants.StartString)))
                    {
                        currentToken += ch;
                        inString = true;
                    }
                    else
                    {
                        currentToken += ch;
                    }
                }
            }

            currentToken = currentToken.Trim();
            if (!string.IsNullOrEmpty(currentToken))
            {
                tokens.Add(currentToken);
            }

            return tokens.ToArray();
        }
    }
}