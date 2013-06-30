using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys = Cosmos.System;
using FAT = Cosmos.System.Filesystem.FAT;
using Cosmos.Hardware.BlockDevice;
using System.IO;
namespace Pulsar.System.Devices {
    public class FileSystem {
        private AtaPio xATA = null;
        private Partition xPartition = null;
        private Sys.Filesystem.FAT.FatFileSystem xFS = null;
        private List<Sys.Filesystem.Listing.Base> FATFilesystemListing;
        public void Start() {
            try {
                xATA = null;
                //Get the AtaPio
                for(int i = 0; i < BlockDevice.Devices.Count; i++) {
                    if(BlockDevice.Devices[i] is AtaPio) {
                        xATA = (AtaPio)BlockDevice.Devices[i];
                    }
                }
                xPartition = null;
                //Get the partition
                for(int i = 0; i < BlockDevice.Devices.Count; i++) {
                    if(BlockDevice.Devices[i] is Partition) {
                        xPartition = (Partition)BlockDevice.Devices[i];
                    }
                }
                xFS = new Sys.Filesystem.FAT.FatFileSystem(xPartition);
                Sys.Filesystem.FileSystem.AddMapping("C", xFS);
                FATFilesystemListing = xFS.GetRoot();
            } catch {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR");
            }
        }
        public String GetDriveType() {
            return (xATA.DriveType == AtaPio.SpecLevel.ATA ? "ATA" : "ATAPI");
        }
        public String GetDeviceSerialNumber() {
            return xATA.SerialNo;
        }
        public String GetDeviceModelNumber() {
            return xATA.ModelNo;
        }
        public ulong GetDeviceBlockSize() {
            return xATA.BlockSize;
        }
        public ulong GetDriveSize() {
            return xATA.BlockCount * xATA.BlockSize / 1024 / 1024;
        }
        public void ListFiles() {
            for(int i = 0; i < FATFilesystemListing.Count; i++) {
                var xItem = FATFilesystemListing[i];
                if(xItem is Sys.Filesystem.FAT.Listing.FatDirectory) {
                    Console.WriteLine("<" + xItem.Name + ">");
                } else if(xItem is Sys.Filesystem.FAT.Listing.FatFile) {
                    Console.WriteLine(xItem.Name + " - " + xItem.Size);
                }
            }
        }
    }
}