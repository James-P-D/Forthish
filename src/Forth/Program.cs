using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Forth.ForthObjects;
using Forth.Run;

namespace Forth
{
    class Program
    {
        // TODO
        // check if not a command/var

        static int Main(string[] args)
        {
            Console.WriteLine(Resources.Program_Main_Forth_Interpreter);
            Console.WriteLine(Resources.Program_Main_Press__ESC__to_exit);
            Console.WriteLine();

            // Load the default files
            LoadResourceFile("Constants");
            LoadResourceFile("Definitions");

            // Load any files inputted from command-line
            foreach (string arg in args)
            {
                LoadSourceFile(arg);
            }

            Console.WriteLine();
            Console.WriteLine(Resources.Program_Main_Ready_);
            Console.WriteLine();

            string currentString = string.Empty;

            // Loop forever. User can only exit through 'quit' or killing the process
            for (;;)
            {
                string newString = Console.ReadLine().Trim();
                if (!IsMultiLine(newString))
                {
                    if (newString.ToLower().Equals(Resources.Program_Main_Quit))
                    {
                        Console.WriteLine();
                        Console.WriteLine(Resources.Program_Main_Exiting);
                        Environment.Exit(0);
                    }
                    else
                    {
                        currentString += " " + newString;
                        Console.Write(Resources.Program_Main__Prompt);
                        if (Executor.ProcessString(currentString))
                        {
                            Console.WriteLine(Resources.Program_Main_OK);
                        }
                        currentString = string.Empty;
                    }
                }
                else
                {
                    currentString += " " + RemoveLastToken(newString);
                }
            }
        }


        /// <summary>
        /// Check to see if input string terminates with a '\' character, which will
        /// signify a multi-line program which should only be processed when user
        /// has finished entering their code
        /// </summary>
        /// <param name="currentString">Current string inputted by user</param>
        /// <returns>True if last token is '\', false otherwise</returns>
        private static bool IsMultiLine(string currentString)
        {
            if (string.IsNullOrEmpty(currentString))
            {
                return false;
            }

            return currentString.EndsWith(Constants.Continuation);
        }

        /// <summary>
        /// Removes last token from string. Used for removing trailing '/' characters
        /// for multi-line input
        /// </summary>
        /// <param name="someString">Current input string</param>
        /// <returns>Current input string minus whatever the last token was</returns>
        private static string RemoveLastToken(string someString)
        {
            string retVal = string.Empty;

            if (string.IsNullOrEmpty(someString))
            {
                return retVal;
            }

            return someString.Substring(0, someString.Length - 1);
        }

        #region Resource loading

        /// <summary>
        /// Loads an internal resource file, typically containing
        /// default Constants, Definitions etc.
        /// </summary>
        /// <param name="resourceName">Name of resource</param>
        private static void LoadResourceFile(string resourceName)
        {
            Console.WriteLine(Resources.Program_LoadResourceFile_Loading_resource___0_, resourceName);
            try
            {
                Executor.ProcessString(Resources.ResourceManager.GetString(resourceName));
            }
            catch (Exception ex)
            {
                Executor.OutputError(string.Format(Resources.ProblemLoadingResource, resourceName));
                Executor.OutputError(ex.Message);
            }
        }

        /// <summary>
        /// Loads a local Forth source code file
        /// </summary>
        /// <param name="sourceFilename">Full path to Forth source code file</param>
        private static void LoadSourceFile(string sourceFilename)
        {
            Console.WriteLine(Resources.Program_LoadSourceFile_Loading_source_file___0_, sourceFilename);
            try
            {
                Executor.ProcessString(File.ReadAllText(sourceFilename));
            }
            catch (Exception ex)
            {
                Executor.OutputError(string.Format(Resources.ProblemLoadingSourceFile, sourceFilename));
                Executor.OutputError(ex.Message);
            }
        }

        #endregion
    }
}