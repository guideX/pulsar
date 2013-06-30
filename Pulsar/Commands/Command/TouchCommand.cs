#region Namespace Imports
using System;
using Pulsar.Environment;
using Pulsar.FileSystem.PulsarFileSystem;
#endregion
namespace Pulsar.Commands {
    public class TouchCommand : CommandBase {
        public TouchCommand()
            : base("TOUCH", 1) {
            Help = "Touch";
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
                return;
            }

            var dir = PulsarDirectory.GetDirectoryByFullName(GlobalEnvironment.Current["CURRENTDIR"]);
            String[] str = args.Split('/');
            if(str.Length > 1) {
                String fulldir = dir.FullName;
                for(int i = 0; i < str.Length - 1; i++) {
                    fulldir += str[i] + "/";
                }
                PulsarDirectory app = PulsarDirectory.GetDirectoryByFullName(fulldir);
                if(app != null && str[str.Length - 1] != null && str[str.Length - 1] != "") {
                    app.AddFile(str[str.Length - 1]);
                }
            } else {
                dir.AddFile(args);
            }
        }
    }
}