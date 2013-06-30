using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Pulsar.System.Devices.Storage {
    public class Drive {
        public enum DriveType {
            HardDrive = 1,
            CD = 2,
            FloppyDrive = 3,
            SolidState = 4,
            Alternate = 5
        }
        public DriveType driveType { get; set; }
        public string driveName { get; set; }
    }
}