using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fs = Cosmos.System.Filesystem;
using Cosmos.Hardware.BlockDevice;
namespace Pulsar.FileSystem.FileSystem {
    class FileSystem : Fs.FileSystem {
        private Byte[] recHash = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e };
        private Partition part;
        #region static Methods And Variables
        public static FileSystem mFS = null;
        public static String separator = "/";
        public static void ClearBlocks(FSBlock StartBlock) {
            FSBlock b = StartBlock;
            while(b.NextBlock != 0) {
                b = FSBlock.Read(b.Partition, b.NextBlock);
                b.Used = false;
                FSBlock .Write(mFS.Partition, b);
            }
        }
        public static void AddMapping(string aPath, FileSystem aFileSystem) {
            mFS = aFileSystem;
        }
        public static string CombineFile(string _Path, string name) {
            String ret = "";
            ret = _Path.TrimEnd(separator.ToCharArray());
            if(ret == null) {
                ret = "";
            }
            if(name != separator) {
                ret += ret += separator + name;
            } else {
                ret = "/";
            }
            return ret;
        }
        public static string CombineDir(string _Path, string name) {
            String ret = "";
            ret = _Path.TrimEnd(separator.ToCharArray());
            if(ret == null) {
                ret = "";
            }
            if(name != separator) {
                ret += separator + name + separator;
            } else {
                ret = "/";
            }
            return ret;
        }
        #endregion
        #region public Properties
        public Partition Partition {
            get {
                return part;
            }
        }
        public PulsarDirectory Root {
            get {
                return new PulsarDirectory(part, 1, separator);
            }
        }
        public ulong BlockSize {
            get {
                return part.BlockSize;
            }
        }
        public ulong BlockCount {
            get {
                return part.BlockCount;
            }
        }
        #endregion
        public FileSystem(Partition p) {
            part = p;
            if(!ISFS()) {
                Helper.WriteLine("");
                Helper.Write("Must clear and rewrite a new fileSystem...");
                if(!CreateNewFS()) {
                    Helper.Error("Cannot create New FileSystem!");
                }
            }
        }
        private bool CreateNewFS() {
            CleanFS(30000);
            Byte[] data = part.NewBlockArray(1);
            recHash.CopyTo(data, 0);
            for(int i = recHash.Length; i < data.Length; i++) {
                data[i] = 0x00;
            }
            part.WriteBlock(0, 1, data);
            return true;
        }
        private bool ISFS() {
            Byte[] data = part.NewBlockArray(1);
            part.ReadBlock(0, 1, data);
            for(int i = 0; i < recHash.Length; i++) {
                if(recHash[i] != data[i]) {
                    return false;
                }
            }
            return true;
        }
        public void CleanFS(ulong stop) {
            Helper.WriteLine("Cleaning FS...");
            Byte[] data = part.NewBlockArray(1);
            for(int j = 0; j < data.Length; j++) {
                data[j] = 0;
            }
            Helper.WriteLine("Starting...");
            uint percent = 0;
            ulong max = part.BlockCount;
            if(stop != 0) {
                max = stop;
            }
            ulong rate = max / 100;
            Helper.WriteLine(percent + "% Done. " + (uint)max + " Blocks Left. ");
            for(ulong i = 0; i < max; i++) {
                part.WriteBlock(i, 1, data);
                if(i % rate == 0) {
                    percent++;
                }
                if(i % 32 == 0) {
                    Helper.WriteOnLastLine(percent + "% Done. " + ((uint)(max - i)) + " Blocks Left ");
                }
            }
            Helper.WriteOnLastLine("100 % Done");
        }
    }
}