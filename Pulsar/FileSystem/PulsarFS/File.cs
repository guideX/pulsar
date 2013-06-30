using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;
namespace Pulsar.FileSystem.FileSystem {
    class File : Entry {
        public String FullName {
            get {
                return FileSystem.CombineDir(_Path, Name);
            }
        }
        public void WriteAllBytes(Byte[] Data) {
            if(_StartBlock.NextBlock != 0) {
                FileSystem.ClearBlocks(_StartBlock);
                _StartBlock.NextBlock = 0;
                FSBlock.Write(FileSystem.mFS.Partition, _StartBlock);
            }
            int index = 0;
            FSBlock curb = FSBlock.GetFreeBlock(FileSystem.mFS.Partition);
            _StartBlock.NextBlock = curb.BlockNumber;
            FSBlock.Write(part, _StartBlock);
            do {
                Byte[] arr = new Byte[FSBlock.MaxBlockContentSize];
                index = Utils.CopyByteToByte(Data, index, arr, 0, arr.Length);
                curb.Used = true;
                curb.Content = arr;
                if(index != Data.Length) {
                    FSBlock b = FSBlock.GetFreeBlock(FileSystem.mFS.Partition);
                    curb.NextBlock = b.BlockNumber;
                    curb.ContentSize = (uint)arr.Length;
                    FSBlock.Write(FileSystem.mFS.Partition, curb);
                    curb = b;
                } else {
                    curb.ContentSize = (uint)(Data.Length % arr.Length);
                    FSBlock.Write(FileSystem.mFS.Partition, curb);
                }
            }
            while(index != Data.Length);
            EditEntryInfo(EntryInfoPosition.DateTimeModified, Environment.DateTime.Now.TimeStamp);
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
        }
        public void WriteAllText(String text) {
            Byte[] b = new Byte[text.Length];
            Utils.CopyCharToByte(text.ToCharArray(), 0, b, 0, text.Length);
            WriteAllBytes(b);
        }
        public Byte[] ReadAllBytes() {
            if(_StartBlock.NextBlock == 0) {
                return new Byte[0];
            }
            FSBlock b = _StartBlock;
            List<Byte> lret = new List<Byte>();
            while(b.NextBlock != 0) {
                b = FSBlock.Read(b.Partition, b.NextBlock);
                for(int i = 0; i < b.ContentSize; i++) {
                    lret.Add(b.Content[i]);
                }
            }
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
            return lret.ToArray();
        }
        public string ReadAllText() {
            Byte[] b = ReadAllBytes();
            Char[] text = new Char[b.Length];
            Utils.CopyByteToChar(b, 0, text, 0, b.Length);
            return Utils.CharToString(text);
        }
        public File(Partition p, ulong bn, String pa) {
            _Path = pa;
            part = p;
            _StartBlock = FSBlock.Read(p, bn);
            if(!_StartBlock.Used) {
                _StartBlock.Used = true;
                String n = "New File";
                CreateEntry(part, _StartBlock, n);
            }
        }
        public override String ToString() {
            return this.Name;
        }
    }
}
