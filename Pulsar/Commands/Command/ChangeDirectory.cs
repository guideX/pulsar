#region Namespace Imports
using System;
using Pulsar.Environment;
using Pulsar.FileSystem.PulsarFileSystem;
#endregion
namespace Pulsar.Commands {
    public class ChangeDirectory : CommandBase {
        public ChangeDirectory()
            : base("CD", 1) {
            Help = "Changes a directory";
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
            if(args == ".")
                return;
            else if(args == "..") {
                GlobalEnvironment.Current["CURRENTDIR"] = GetHomeDir(GlobalEnvironment.Current["CURRENTDIR"]);
            } else {
                var dir = PulsarDirectory.GetDirectoryByFullName(GlobalEnvironment.Current["CURRENTDIR"]);
                var name = args;
                var d = dir.GetDirectoryByName(name);

                if(d == null)
                    d = PulsarDirectory.GetDirectoryByFullName(name);

                if(d == null)
                    Helper.WriteLine("Can't find path");
                else
                    GlobalEnvironment.Current["CURRENTDIR"] = d.FullName;
            }
        }
        private string GetHomeDir(string dir) {
            if(dir == "/")
                return "/";
            else {
                var a = dir.Split('/');
                String homedir = "/";
                for(int i = 0; i < a.Length - 2; i++) {
                    homedir += a[i] + "/";
                }
                return homedir;
            }
        }
    }
}