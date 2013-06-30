using System;
namespace Pulsar.Commands {
    class HelpCommand : CommandBase {
        public HelpCommand() : base("HELP", 0) {
            Help = "Displays available list of commands - type 'Help command' for specific help";
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
                var InternalCommands = CommandManager.GetCommands();
                for(int i = 0; i < InternalCommands.Length; i++) {
                    if(InternalCommands[i] != null && InternalCommands[i].Help != null && InternalCommands[i].Name != null) {
                        Helper.WriteLine(InternalCommands[i].Name.ToLower() + " - " + InternalCommands[i].Help, ConsoleColor.Blue);
                    }
                }
            } else {
                CommandBase c = CommandManager.GetCommand(args);
                if(c == null) {
                    Helper.WriteLine("No such command found!");
                } else {
                    if(c.Help != null) {
                        Helper.WriteLine(c.Name + " - " + c.Help, ConsoleColor.Blue);
                    }
                }
            }
        }
    }
}