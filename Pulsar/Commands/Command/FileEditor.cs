using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pulsar.FileSystem.PulsarFileSystem;
namespace Pulsar.Commands {
    class FileEditor : CommandBase {
        public FileEditor() :
            base("editor", 1) {
            Help = "Allows you to edit any file";
            AccessRights = AccessRights.Everyone;
        }
        public override string Help {
            get;
            set;
        }
        public override AccessRights AccessRights {
            get;
            set;
        }
        public override bool CanExecute(String args) {
            return true;
        }
        public override void Execute(String args) {
            Helper.WriteLine("Loading File Editor v0.1");
        }
    }
}