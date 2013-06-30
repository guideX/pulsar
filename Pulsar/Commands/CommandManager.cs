#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pulsar.FileSystem.PulsarFileSystem;
#endregion
namespace Pulsar.Commands {
    public static class CommandManager {
        #region Ctor
        static CommandManager() {
            cmds = new List<CommandBase>();
        }
        #endregion
        #region Implementations
        public static bool Register(CommandBase cmd) {
            if(cmd == null) {
                return false;
            }
            Boolean reg = Find(cmd.Name) != null;
            if(reg) {
                return false;
            }
            cmds.Add(cmd);
            return true;
        }
        public static void UnRegister(CommandBase cmd) {
            cmds.Remove(cmd);
        }
        public static bool Contains(CommandBase cmd) {
            return cmds.Contains(cmd);
        }
        public static CommandBase GetCommand(String name) {
            return Find(name);
        }
        private static CommandBase Find(String cmd) {
            for(int i = 0; i < cmds.Count; i++) {
                if(cmds[i].Name.ToLower().IsEqual(cmd.ToLower()))
                    return cmds[i];
            }
            return null;
        }
        public static bool ProcessCommand(String cmd, String args) {
            CommandBase c = Find(cmd);
            if(c == null) {
                PulsarDirectory dir = PulsarDirectory.GetDirectoryByFullName(Pulsar.Environment.GlobalEnvironment.Current["CURRENTDIR"]);
                if(dir != null) {
                    PulsarFile fl = dir.GetFileByName(cmd);
                    if(fl != null) {
                        Helper.WriteLine("Please make sure the file you have selected IS A BINARY or expect lots of crashes!");
                        if(Helper.Continue()) {
                            //Pulsar.Executables.BinaryLoader.CallRaw(fl.ReadAllBytes());
                            return true;
                        }
                    }
                }
                return false;
            }
            if(c.CanExecute(args))
                c.Execute(args);
            return true;
        }
        public static CommandBase[] GetCommands() {
            return cmds.ToArray();
        }
        public static ReadOnlyCollection<CommandBase> Commands {
            get {
                return new ReadOnlyCollection<CommandBase>(GetCommands());
            }
        }
        #endregion
        #region Fields
        public static List<CommandBase> cmds = null;
        #endregion
    }
}