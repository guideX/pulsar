#region Namespace Imports
using System;
using Pulsar.Environment;
using Pulsar.FileSystem.PulsarFileSystem;
#endregion
namespace Pulsar.Commands {
    public class MakeDirectory : CommandBase {
        #region Ctor
        public MakeDirectory()
            : base("mkdir", 1) {
            Help = "Creates a directoty";
            AccessRights = AccessRights.Everyone;
        }
        #endregion
        #region Memebers
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
            if(args == null || args.Length == NumArgs) {
                Helper.WriteLine("Either no or invalid arguments specified!");
                return;
            }
            var curDir = PulsarDirectory.GetDirectoryByFullName(GlobalEnvironment.Current["CURRENTDIR"]);
            String dirName = args;
            String[] str = dirName.Split('/');
            if(str.Length > 1) {
                String fulldir = curDir.FullName;
                for(int i = 0; i < str.Length - 1; i++) {
                    fulldir += str[i] + "/";
                }
                PulsarDirectory app = PulsarDirectory.GetDirectoryByFullName(fulldir);
                if(app != null && str[str.Length - 1] != null && str[str.Length - 1] != "") {
                    app.AddDirectory(str[str.Length - 1]);
                }
            } else {
                curDir.AddDirectory(dirName);
            }
        }

        #endregion
    }
}