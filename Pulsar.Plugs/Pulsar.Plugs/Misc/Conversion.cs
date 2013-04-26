using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Pulsar.Plugs.Misc {
  public class Misc {
    public Boolean IsNullOrEmpty(string _Data) {
      return string.IsNullOrEmpty(_Data);
    }
    public int ConvertStringToInt(string _String) {
      try {
        int n;
        n = Convert.ToInt32(_String);
        return n;
      }
      catch (FormatException e) {
        //if (lVerbose == true) {
          //ShowError("Not Numeric [" + e.Message + "]");
        //}
        return 0;
      }
      catch (OverflowException e) {
        //if (lVerbose == true) {
          //ShowError("Too big for an Int32 [" + e.Message + "]");
        //}
        return 0;
      }
    }

  }
}
