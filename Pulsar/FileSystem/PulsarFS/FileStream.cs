using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Pulsar.FileSystem.FileSystem {
    class FileStream {
        private File CurrentFile;
        public String Name {
            get {
                return CurrentFile.Name;
            }
        }
        public FileStream(String Name) {
        }
    }
}