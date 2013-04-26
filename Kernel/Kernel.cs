// Pulsar Operating System
// Version 0.1 - Leon Aiossa
using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
namespace Pulsar {
  public class Kernel : Sys.Kernel {
    clsCommandLineCommands lCommands;
    protected override void BeforeRun() {
      Console.Clear();
      lCommands.ShowAbout();
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.WriteLine("Math Commands: Add Numbers, Abs, Verbose, Acos, Add Doubles");
      Console.WriteLine("General Commands: Add Variable, About, Test");
      Console.WriteLine("Socket: Connect, SendMessage");
    }
    public static string Right(string text, int length) {
      if (length < 0) {
        
        return text.Substring(text.Length - length);
      } else {
        return text.Substring(0, length);
      }
    }
    public static string Left(string text, int length) {
      if (length < 0)
        return "";
      else if (length == 0 || text.Length == 0)
        return "";
      else if (text.Length <= length)
        return text;
      else
        return text.Substring(0, length);
    }
    protected override void Run() {
      lCommands = new clsCommandLineCommands();
      string _Command;
      string _Command2;
      string _Command3;
      Boolean b = false;
      Boolean b2 = false;
      while (true) {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Pulsar> ");
        Console.ForegroundColor = ConsoleColor.White;
        _Command = Console.ReadLine();
        if (_Command == "About") {
          lCommands.ShowAbout();
          b = true;
        }
        if (_Command == "Edit") {
          while(b2 == false) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Edit v1.0");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            _Command2 = Console.ReadLine();
            if(_Command2.Length == 0) {
              b2 = true;
            }
          }

        }
        // Display verbose values or not
        if (_Command == "Verbose") {
          lCommands.ToggleVerbose();
          b = true;
        }
        // Create a variable
        if (_Command == "Connect") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command2 = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command3 = Console.ReadLine();
            b = true;
            b2 = true;
          }
        }
        if (_Command == "Connect") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command2 = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("# " + _Command2 + " = ");
            _Command3 = Console.ReadLine();
            //lCommands.Connect(_Command2, _Command3);
            b = true;
            b2 = true;
          }
        }
        if (_Command == "Send Message") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            _Command2 = Console.ReadLine();
            //lCommands.SendMessage(_Command2);
            b = true;
            b2 = true;
          }
        }
        if (_Command == "Add Variable") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command2 = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> " + _Command2 + " = ");
            _Command3 = Console.ReadLine();
            lCommands.AddVariable(_Command2, _Command3);
            b = true;
            b2 = true;
          }
        }
        #region "MATH"
        // Add two doubles together
        if (_Command == "Add Doubles") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("# ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command2 = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("# " + _Command2 + " + ");
            _Command3 = Console.ReadLine();
            lCommands.AddDoubles(_Command2, _Command3);
            b = true;
            b2 = true;
          }
        }
        // Add Numbers
        if (_Command == "Add Numbers") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("# ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command2 = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("# " + _Command2 + " + ");
            _Command3 = Console.ReadLine();
            lCommands.AddNumbers(_Command2, _Command3);
            b = true;
            b2 = true;
          }
        }
        // Returns the absolute value
        if (_Command == "Abs") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("# ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command2 = Console.ReadLine();
            lCommands.Abs(_Command2);
            b = true;
            b2 = true;
          }
        }
        // Returns the angle whose cosine is the specified number.
        if (_Command == "Acos") {
          while (b2 == false) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("# ");
            Console.ForegroundColor = ConsoleColor.White;
            _Command2 = Console.ReadLine();
            lCommands.Acos(_Command2);
            b = true;
            b2 = true;
          }
        }
        #endregion
        //lCommands.CheckSocketData();
        if (b == false) {
          lCommands.ShowUnknownCommand(_Command);
        }
        b = false;
        b2 = false;
      }
    }
  }
}