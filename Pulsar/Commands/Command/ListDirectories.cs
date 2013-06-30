#region Namespace Imports
using System;
using Pulsar.Environment;
using Pulsar.FileSystem.PulsarFileSystem;
#endregion
namespace Pulsar.Commands {
    public class ListDirectories : CommandBase {
        public ListDirectories() : base("LS", 0) {
            Help = "Retrives the list of directories";
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
            var dir = PulsarDirectory.GetDirectoryByFullName(GlobalEnvironment.Current["CURRENTDIR"]);
            var dirs = dir.GetDirs();
            var files = dir.GetFiles();
            dirs.ForEeach(d => Helper.Write(d.ToString() + " ", ConsoleColor.Blue));
            files.ForEeach(f => Helper.Write(f.ToString() + " ", ConsoleColor.Green));
        }
    }
}