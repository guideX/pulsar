using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsar.System.Math {
    public class Math {
        public static int IntLength(int i) {
            if(i < 0)
                throw new ArgumentOutOfRangeException();
            if(i == 0)
                return 1;
            return (int)Math.Floor(Math.Log10(i)) + 1;
        }
    }
}
