using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Core;
using Cosmos.Hardware;
using Cosmos.Hardware.BlockDevice;
using Cosmos.System.Filesystem.Listing;
using Pulsar.FileSystem.Physical;
using Pulsar.FileSystem.PulsarFileSystem;
using Pulsar.FileSystem.Physical.Drivers;
namespace Pulsar {
    public class Init {
        public static bool Error = false;
        private static PrimaryPartition workPartition = null;
        public static int RunInit() {
            #region Version
            Helper.WriteLine("Pulsar 0.1", ConsoleColor.Blue);
            #endregion
            #region Memory
            uint mem = CPU.GetAmountOfRAM();
            Helper.Write("Memory: " + (mem + 2) + " MB ");
            Helper.WriteLine("OK", ConsoleColor.Green);
            #endregion
            #region Disks and Partitions
            IDE[] IDEs = IDE.Devices.ToArray();
            Helper.WriteLine("Number of IDE disks: " + IDEs.Length);
            for(int i = 0; i < IDEs.Length; i++) {
                PrimaryPartition[] parts = IDEs[i].PrimaryPartitions;
                for(int j = 0; j < parts.Length; j++) {
                    if(parts[j].Infos.SystemID == 0xFA) {
                        workPartition = parts[j];
                    }
                }
            }
            if(workPartition == null) {
                DiskHandler.CreatePartitions(IDEs);
                Helper.WriteLine("The machine needs to be restarted.");
                return 2;
            }
            //Helper.Done();
            #endregion
            #region FileSystem
            PulsarFileSystem fs;
            try {
                fs = new PulsarFileSystem(workPartition);
                PulsarFileSystem.AddMapping("/", fs);
                Helper.Done();
            } catch(Exception ex) {
                Helper.Error("Error!" + ex.Message);
                Error = true;
            }
            #endregion
            #region Installation
            if(PulsarFileSystem.mFS.Root.GetDirectoryByName("etc") == null) {
                Helper.WriteLine("Welcome to Pulsar!");
                /*Helper.WriteLine("The basic directories needed for running are not present.");
                Helper.WriteLine("If you're newly installing Pulsar, this is normal, otherwise, you've probably deleted something bad");
                Helper.WriteLine("This will delete all file system contents.");
                if (!Helper.Continue())
                {
                    return 1;
                }*/
                Helper.Write("Creating Pulsar System ...");
                PulsarEntry[] entries = PulsarFileSystem.mFS.Root.GetEntries();
                for(int i = 0; i < entries.Length; i++) {
                    if(entries[i] is PulsarDirectory) {
                        PulsarFileSystem.mFS.Root.RemoveDirectory(entries[i].Name);
                    }
                    if(entries[i] is PulsarFile) {
                        PulsarFileSystem.mFS.Root.RemoveFile(entries[i].Name);
                    }
                }
                Helper.Done();
                //Create Directories and check them
                try {
                    CreateDirectoryAndVerify("etc");
                    CreateDirectoryAndVerify("bin");
                    CreateDirectoryAndVerify("sbin");
                    CreateDirectoryAndVerify("proc");
                    CreateDirectoryAndVerify("usr");
                    CreateDirectoryAndVerify("home");
                    CreateDirectoryAndVerify("root");
                    CreateDirectoryAndVerify("tmp");
                    CreateDirectoryAndVerify("var");
                    CreateDirectoryAndVerify("srv");
                    CreateDirectoryAndVerify("lib");
                    CreateDirectoryAndVerify("opt");
                    CreateDirectoryAndVerify("dev");
                    CreateFileAndVerify("passwd", "etc");
                    CreateFileWithContentsAndVerify("motd", "etc", "Welcome to Pulsar 0.1!\nEdit this MOTD in /etc/motd.\nThanks for using Pulsar!");
                    CreateFileWithContentsAndVerify("witcher", "bin", new byte[] { 0xB0, 0x03, 0x66, 0xBB, 0x11, 0x00, 0x00, 0x00, 0x66, 0xB9, 0x07, 0x00, 0x00, 0x00, 0xCD, 0x80, 0xC3, 0x57, 0x69, 0x74, 0x63, 0x68, 0x21, 0x0A });
                } catch(Exception e) {
                    Helper.Error("Error! " + e.Message);
                    return 1;
                }
                /*PulsarFileSystem.mFS.Root.AddDirectory("inf");
                PulsarFileSystem.mFS.Root.AddDirectory("bin");
                PulsarDirectory infdir = PulsarFileSystem.mFS.Root.GetDirectoryByName("inf");
                if (infdir == null)
                {
                    Helper.Error("Cannot create required directories...");
                    Helper.WriteLine("Aborting...");
                    return 1;
                }
                PulsarDirectory bindir = PulsarFileSystem.mFS.Root.GetDirectoryByName("bin");
                if (bindir == null)
                {
                    Helper.Error("Cannot create required directories...");
                    Helper.WriteLine("Aborting...");
                    return 1;
                }
                infdir.AddFile("Accounts");
                //Create Files and check them
                PulsarFile Accfile = infdir.GetFileByName("Accounts");
                if (Accfile == null)
                {
                    Helper.Error("Cannot create required files...");
                    Helper.WriteLine("Aborting...");
                    return 1;
                }
                bindir.AddFile("witcher");
                PulsarFile Witchfile = bindir.GetFileByName("witcher");
                if (Witchfile == null)
                {
                    Helper.Error("Cannot create required files...");
                    Helper.WriteLine("Aborting...");
                    return 1;
                }
                else
                {
                    Witchfile.WriteAllBytes();
                }*/
                //Helper.Done();
                String newus;
                String newpass;
                do {
                    Helper.WriteLine("New user required");
                    newus = Helper.ReadLine("Username: ");
                    Helper.Write("Password: ");
                    newpass = Helper.ReadLine(true);
                } while(!Security.Account.Add(newus, newpass));
                Helper.NoError("User Added Succesfully");
            }
            #endregion
            return 0;
        }
        private static PulsarDirectory sf;
        private static void CreateDirectoryAndVerify(string directory) {
            PulsarFileSystem.mFS.Root.AddDirectory(directory);
            sf = PulsarFileSystem.mFS.Root.GetDirectoryByName(directory);
            if(sf == null) {
                Helper.Error("Cannot create required files, aborting creation of " + directory);
                throw new Exception("Failed creation of directories.");
            }
        }
        private static void CreateFileAndVerify(string file, string directory) {
            PulsarDirectory dir = PulsarFileSystem.mFS.Root.GetDirectoryByName(directory);
            if(dir != null) {
                dir.AddFile(file);
                PulsarFile nf = dir.GetFileByName(file);
                if(nf == null) {
                    throw new Exception("Could not create!");
                }
            } else {
                throw new ArgumentException("Bad directory");
            }
        }
        private static void CreateFileWithContentsAndVerify(string file, string directory, byte[] data) {
            PulsarDirectory dir = PulsarFileSystem.mFS.Root.GetDirectoryByName(directory);
            if(dir != null) {
                dir.AddFile(file);
                PulsarFile nf = dir.GetFileByName(file);
                if(nf == null) {
                    throw new Exception("Could not create!");
                } else {
                    nf.WriteAllBytes(data);
                }

            } else {
                throw new ArgumentException("Bad directory");
            }
        }
        private static void CreateFileWithContentsAndVerify(string file, string directory, string data) {
            PulsarDirectory dir = PulsarFileSystem.mFS.Root.GetDirectoryByName(directory);
            if(dir != null) {
                dir.AddFile(file);
                PulsarFile nf = dir.GetFileByName(file);
                if(nf == null) {
                    throw new Exception("Could not create!");
                } else {
                    nf.WriteAllText(data);
                }

            } else {
                throw new ArgumentException("Bad directory");
            }
        }
    }

}