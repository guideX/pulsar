using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pulsar.FileSystem;
using Cosmos.Core;
using Pulsar.FileSystem.PulsarFileSystem;
using Pulsar.Environment;
using Pulsar.Commands;
namespace Pulsar {
    class Shell {
        /// <summary>
        /// Starts a new infinite loop that represents a simple shell
        /// </summary>
        public static void StartShell() {
            Helper.WriteLine("Starting Shell...", ConsoleColor.Green);
            PulsarFile motd = PulsarDirectory.GetFileByFullName("/etc/motd");
            if(motd == null) {
                Helper.Error("MOTD not available. Please type touch /etc/motd in the command line!");
            } else {

                Helper.WriteLine(motd.ReadAllText());
            }
            String cmd = "";
            do {
                Security.Account u = new Security.Account(GlobalEnvironment.Current["USER"]);
                String h = GlobalEnvironment.Current["HOST"];
                PulsarDirectory dir = PulsarDirectory.GetDirectoryByFullName(GlobalEnvironment.Current["CURRENTDIR"]);
                String consoleline = u.Username + "@" + h + ":" + dir.FullName + "# ";
                Helper.Write(consoleline);

                cmd = Helper.ReadLine();
                String[] t = cmd.SplitAtFirstSpace();
                String c = t[0]; // Command
                String a = t.Length > 1 ? t[1] : String.Empty; // Argument(s)

                Boolean p = CommandManager.ProcessCommand(c, a);

                if(!p)
                    Helper.WriteLine("No such command found!");

                if(!Helper.LastLineIsEmpty()) {
                    Helper.WriteLine("");
                }
            }
            while(true);
        }
    }
}