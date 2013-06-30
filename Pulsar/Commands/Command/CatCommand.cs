#region Namespace Imports
using System;
using Pulsar.Environment;
using Pulsar.FileSystem.PulsarFileSystem;
#endregion
namespace Pulsar.Commands {
    public class CatCommand : CommandBase {
        public CatCommand()
            : base("CAT", 1) {
            Help = "Cat";
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
                Helper.WriteLine("target File non specified");
                return;
            }
            var dir = PulsarDirectory.GetDirectoryByFullName(GlobalEnvironment.Current["CURRENTDIR"]);
            PulsarFile file = dir.GetFileByName(args);
            if(file == null) {
                file = PulsarDirectory.GetFileByFullName(args);
            }
            if(file == null) {
                Helper.WriteLine("Can't find file");
            } else {
                String content = file.ReadAllText();
                Helper.WriteLine(content);
            }
        }
    }
}