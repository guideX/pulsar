using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sys = Cosmos.System;
//using Pulsar.Plugs;
//using Pulsar.Plugs.Communications;
//using Pulsar.Plugs.Misc;
namespace Pulsar {
  class clsVariable {
    public string vName;
    public string vValue;
  }
  class clsCommandLineCommands {
    //SocketController lSocket = new SocketController();
    //Misc lMisc = new Misc();
    private Boolean lVerbose;
    List<clsVariable> lVariables = new List<clsVariable>();
    public void InitKernel() {
      try {
        //InitSocket();
      }
      catch (InvalidOperationException ex) {
        ShowError(ex.Message);
      }
    }
    public void AddVariable(string _Name, string _Value) {
      try {
        clsVariable _Variable = new clsVariable();
        _Variable.vName = _Name;
        _Variable.vValue = _Value;
        lVariables.Add(_Variable);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Add Variable[ " + _Variable + ": + " + _Value + " ]");
        Console.ForegroundColor = ConsoleColor.Yellow;
      }
      catch (InvalidOperationException ex) {
        if (lVerbose == true) {
          ShowError(ex.Message);
        }
      }
    }
    public void ToggleVerbose() {

      if (lVerbose == true) {
        lVerbose = false;
        ShowInformational("Verbose", "False");
      }
      else {
        lVerbose = true;
        ShowInformational("Verbose", "True");
      }
    }
    public double ConvertStringToDouble(string _String) {
      try {
        double d;
        byte[] _Bytes = new byte[_String.Length];
        for (int i = 0; i < _String.Length; i++) _Bytes[i] = (byte)(int)_String[i];
        d = BitConverter.ToDouble(_Bytes, 0);
        return d;
      }
      catch (InvalidOperationException ex) {
        if (lVerbose == true) {
          ShowError(ex.Message);
        }
        return 0.0;
      }
    }
    public void ShowError(string _Command) {
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.WriteLine(" ");
      Console.WriteLine("-");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write("Error [");
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.Write(_Command);
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("]");
    }
    public void ShowUnknownCommand(string _Command) {
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.WriteLine("-");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write("Unknown Command [");
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.Write(_Command);
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("]");
    }
    public void ShowInformational(string _Informational, string _Value) {
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.WriteLine("-");
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write(_Informational + " [");
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.Write(_Value);
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("]");
    }
    public void ShowAbout() {
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.WriteLine("-");
      Console.ForegroundColor = ConsoleColor.Blue;
      Console.WriteLine("Pulsar Operating System");
      Console.WriteLine("Version: 0.1");
    }
    #region "MATH"
    public void AddNumbers(string _Value, string _AddValue) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write("Add[ " + _Value + " + " + _AddValue + " ] = ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      int n = 0;
      int e = 0;
      try {
        n = sys.Plugs.System.Int32Impl.Parse(_Value);
        e = sys.Plugs.System.Int32Impl.Parse(_AddValue);
      }
      catch (InvalidOperationException ex) {
        if (lVerbose == true) {
          ShowError(ex.Message);
        }
      }
      int t = n + e;
      Console.WriteLine(t);
    }
    public void AddDoubles(string _Value, string _AddValue) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write("Add[ " + _Value + " + " + _AddValue + " ] = ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      double n = 0.0;
      double e = 0.0;
      try {
        n = ConvertStringToDouble(_Value);
        e = ConvertStringToDouble(_AddValue);
      }
      catch (InvalidOperationException ex) {
        if (lVerbose == true) {
          ShowError(ex.Message);
        }
      }
      double t = n + e;
      Console.WriteLine(t);
    }
    public void Abs(string _Value) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write("Abs[ " + _Value + " ] = ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      try {
        double d;
        d = ConvertStringToDouble(_Value);
        Console.WriteLine(Cosmos.System.Plugs.System.MathImpl.Abs(d));
      }
      catch (InvalidOperationException ex) {
        if (lVerbose == true) {
          ShowError(ex.Message);
        }
      }
    }
    public void Acos(string _Value) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write("Acos[ " + _Value + " ] = ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      try {
        double d;
        d = ConvertStringToDouble(_Value);
        Console.WriteLine(Cosmos.System.Plugs.System.MathImpl.Acos(d));
      }
      catch (InvalidOperationException ex) {
        if (lVerbose == true) {
          ShowError(ex.Message);
        }
      }
    }
    #endregion
  }
}