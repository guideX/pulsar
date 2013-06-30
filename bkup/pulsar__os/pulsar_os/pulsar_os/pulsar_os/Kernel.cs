using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
namespace pulsar_os {
    public class Kernel : Sys.Kernel {
        System.FileSystem fileSystem;
        protected override void BeforeRun() {
            Console.WriteLine("Pulsar 0.1 - Alpha");
            fileSystem = new System.FileSystem();
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