#region Namespace Imports
using System;
using Pulsar.Environment;
using Pulsar.FileSystem.PulsarFileSystem;
#endregion
namespace Pulsar.Commands {
    public class RemoveDirectory : CommandBase {
        public RemoveDirectory()
            : base("rm", 1) {
            Help = "Removes a directory and files from the disk";
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
            if(String.IsNullOrEmpty(args)) {
                Helper.WriteLine("Either no or invalid arguments specified!");
            } else {
                String file = args;
                PulsarDirectory dir = PulsarDirectory.GetDirectoryByFullName(GlobalEnvironment.Current["CURRENTDIR"]);
                dir.RemoveFile(file);
                dir.RemoveDirectory(file);
            }
        }
    }
}