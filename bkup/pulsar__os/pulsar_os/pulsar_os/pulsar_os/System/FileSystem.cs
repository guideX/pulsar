using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys = Cosmos.System;
using FAT = Cosmos.System.Filesystem.FAT;
using Cosmos.Hardware.BlockDevice;
using System.IO;
namespace pulsar_os.System {
    public class FileSystem {
        private AtaPio ataPio = null;
        private Partition partition = null;
        private Sys.Filesystem.FAT.FatFileSystem fatFileSystem = null;
        private List<Sys.Filesystem.Listing.Base> fatFileSystemListing;
        public FileSystem() {
            try {
                ataPio = null;
                for(int i = 0; i < BlockDevice.Devices.Count; i++) {
                    if(BlockDevice.Devices[i] is AtaPio) {
                        ataPio = (AtaPio)BlockDevice.Devices[i];
                    }
                }
                partition = null;
                for(int i = 0; i < BlockDevice.Devices.Count; i++) {
                    if(BlockDevice.Devices[i] is Partition) {
                        partition = (Partition)BlockDevice.Devices[i];
                    }
                }
                fatFileSystem = new Sys.Filesystem.FAT.FatFileSystem(partition);
                Sys.Filesystem.FileSystem.AddMapping("C", fatFileSystem);
                fatFileSystemListing = fatFileSystem.GetRoot();
            } catch (Exception ex) {
                throw ex;
            }
        }
        public String GetDriveType() {
            return (ataPio.DriveType == AtaPio.SpecLevel.ATA ? "ATA" : "ATAPI");
        }
        public String GetDeviceSerialNumber() {
            return ataPio.SerialNo;
        }
        public String GetDeviceModelNumber() {
            return ataPio.ModelNo;
        }
        public ulong GetDeviceBlockSize() {
            return ataPio.BlockSize;
        }
        public ulong GetDriveSize() {
            return ataPio.BlockCount * ataPio.BlockSize / 1024 / 1024;
        }
        public void ListFiles() {
            for(int i = 0; i < fatFileSystemListing.Count; i++) {
                Sys.Filesystem.Listing.Base fatItem = fatFileSystemListing[i];
                if(fatItem is Sys.Filesystem.FAT.Listing.FatDirectory) {
                    Console.WriteLine("<DIR> " + fatItem.Size + " " + fatItem.Name);
                } else if(fatItem is Sys.Filesystem.FAT.Listing.FatFile) {
                    Console.WriteLine("     " + fatItem.Size + " " + fatItem.Name);
                }
            }
        }
    }
}