using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Forth
{
  class Program
  {
    // TODO
    // check if not a command/var
    
    static void Main(string[] args)
    {
      Console.WriteLine("Forth Interpreter");
      Console.WriteLine("Press <ESC> to exit");
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
      Console.WriteLine("Ready!");
      Console.WriteLine();

      // Hide the cursor
      Console.CursorVisible = false;

      // Tedious stuff for manually handling Console Application keypresses 
      int index = 0;
      string currentString = string.Empty;
      string multiLineString = string.Empty;
      List<string> previousCommands = new List<string>();
      int previousCommandPointer = -1;

      // Loop forever. User can only exit through <ESC> or killing the process
      for (; ; )
      {
        ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

        switch (consoleKeyInfo.Key)
        {
          case ConsoleKey.UpArrow:
            if (previousCommandPointer > 0)
            {
              previousCommandPointer--;
              currentString = previousCommands[previousCommandPointer];
              index = currentString.Length;
              UpdateConsole(currentString);
            }

            break;
          case ConsoleKey.DownArrow:
            if (previousCommandPointer < previousCommands.Count - 1)
            {
              previousCommandPointer++;
              currentString = previousCommands[previousCommandPointer];
              index = currentString.Length;
              UpdateConsole(currentString);
            }
            break;
          case ConsoleKey.LeftArrow:
            if (index > 0)
            {
              index--;
            }
            break;
          case ConsoleKey.RightArrow:
            if (index < currentString.Length)
            {
              index++;
            }
            break;
          case ConsoleKey.Enter:
            string trimmedCurrentString = currentString.Trim();

            if (!string.IsNullOrEmpty(trimmedCurrentString))
            {
              if (!previousCommands.Contains(trimmedCurrentString))
              {
                previousCommands.Add(trimmedCurrentString);
                previousCommandPointer = previousCommands.Count;
              }

              if (IsMultiLine(trimmedCurrentString))
              {
                trimmedCurrentString = RemoveLastToken(trimmedCurrentString);
                multiLineString += " " + trimmedCurrentString;

                Console.WriteLine();
              }
              else
              {
                multiLineString += " " + trimmedCurrentString;

                Console.WriteLine();
                Console.Write("> ");
                if (Executor.ProcessString(multiLineString))
                {
                  Console.WriteLine("OK");
                }
                multiLineString = string.Empty;
              }

              currentString = string.Empty;
              index = 0;
            }
            break;
          case ConsoleKey.Backspace:
            if (index > 0)
            {
              index--;
              currentString = currentString.Remove(index, 1);
              UpdateConsole(currentString);
            }
            break;
          case ConsoleKey.Delete:
            if (currentString.Length > 0)
            {
              currentString = currentString.Remove(index, 1);
              UpdateConsole(currentString);
            }
            break;
          case ConsoleKey.Escape:
            {
              Console.WriteLine();
              Console.WriteLine("Exiting");
              Environment.Exit(0);
              break;
            }
          default:
            currentString = currentString.Insert(index, consoleKeyInfo.KeyChar.ToString());
            index++;

            UpdateConsole(currentString);
            break;
        }
      }
    }

    /// <summary>
    /// Simple method to clear the current line on the Console and replace it with currentString
    /// </summary>
    /// <param name="currentString">Text to display on Console</param>
    private static void UpdateConsole(string currentString)
    {
      int currentLineCursor = Console.CursorTop;
      Console.SetCursorPosition(0, Console.CursorTop);
      Console.Write(new string(' ', Console.WindowWidth));
      Console.SetCursorPosition(0, currentLineCursor);
      Console.Write(currentString);
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

      if (currentString.Split(new char[] { ' ', '\t' }).Last().Equals(Constants.CONTINUATION))
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Removes last token from string. Used for removing trailing '/' characters
    /// for multi-line input
    /// </summary>
    /// <param name="currentString">Current input string</param>
    /// <returns>Current input string minus whatever the last token was</returns>
    private static string RemoveLastToken(string currentString)
    {
      string retVal = string.Empty;

      if (string.IsNullOrEmpty(currentString))
      {
        return retVal;
      }

      string[] tokens = currentString.Split(new char[] { ' ', '\t' });
      for (int i = 0; i < tokens.Count() - 1; i++)
      {
        retVal += tokens[i] + " ";
      }

      return retVal;
    }

    #region Resource loading

    /// <summary>
    /// Loads an internal resource file, typically containing
    /// default Constants, Definitions etc.
    /// </summary>
    /// <param name="resourceName">Name of resource</param>
    private static void LoadResourceFile(string resourceName)
    {
      Console.WriteLine("Loading resource: {0}", resourceName);
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
      Console.WriteLine("Loading source file: {0}", sourceFilename);
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