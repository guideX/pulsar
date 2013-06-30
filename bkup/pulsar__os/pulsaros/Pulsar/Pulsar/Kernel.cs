using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Filesystem;

namespace Pulsar {

    public class Kernel : Sys.Kernel {
        Pulsar.System.Devices.FileSystem fileSystem = new Pulsar.System.Devices.FileSystem();
        protected override void BeforeRun() {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            fileSystem.Start();
            fileSystem.ListFiles();
            
        }

        protected override void Run() {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            Console.Write("Text typed: ");
            Console.WriteLine(input);
        }
    }
}
