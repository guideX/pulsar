using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;
using System.ComponentModel;
namespace Pulsar.FileSystem.FileSystem {
    enum EntryInfoPosition {
        DateTimeCreated = 0x00, //+8
        DateTimeModified = 0x08, //+8
        DateTimeLastAccess = 0x10, //+8
        Owner = 0x18, //+8
        Group = 0x20, //+8
        Visible = 0x28, //+1
        Writable = 0x29, //+1
        OwnerPermissions = 0x30, //Could be set better
        GroupPermissions = 0x3a, //Could be set better
        EveryonePermissions = 0x3b //Could be set better
    }
    class Entry {
        protected static Char[] InvalidChars = new Char[] { '/', '?', '*' };
        protected FSBlock _StartBlock;
        protected Partition part;
        protected String _Path;
        private static int MaxNameSize = 255;
        private int CustomSize = (int)(FSBlock.MaxBlockContentSize - MaxNameSize);
        public FSBlock StartBlock {
            get {
                return _StartBlock;
            }
        }
        public String Path {
            get {
                return _Path;
            }
        }
        public String Name {
            get {
                Byte[] arr = _StartBlock.Content;
                String str = "";
                for(int i = 0; i < MaxNameSize; i++) {
                    if(arr[i] == 0)
                        break;
                    str += ((Char)arr[i]).ToString();
                }
                return str;
            }
        }
        public void EditEntryInfo(EntryInfoPosition pos, long value) {
            if(pos < EntryInfoPosition.Visible) {
                Utils.CopyByteToByte(BitConverter.GetBytes(value), 0, _StartBlock.Content, (int)MaxNameSize + (int)pos, 8, false);
            } else {
                Utils.CopyByteToByte(BitConverter.GetBytes(value), 0, _StartBlock.Content, (int)MaxNameSize + (int)pos, 1, false);
            }
        }
        protected static FSBlock CreateEntry(Partition p, String name) {
            return CreateEntry(p, FSBlock.GetFreeBlock(p), name);
        }
        protected static FSBlock CreateEntry(Partition p, FSBlock b, String n) {
            if(b != null && ((!Utils.StringContains(n, InvalidChars)) || b.BlockNumber == 0)) {
                b.Used = true;
                b.NextBlock = 0;
                b.TotalSize = 0;
                Char[] nm = n.ToCharArray();
                for(int i = 0; i < nm.Length; i++) {
                    b.Content[i] = (byte)nm[i];
                }
                if(b.BlockNumber != 0) {
                    Utils.CopyByteToByte(BitConverter.GetBytes(Environment.DateTime.Now.TimeStamp), 0, b.Content, (int)MaxNameSize + (int)EntryInfoPosition.DateTimeCreated, 8, false);
                }
                b.Content[nm.Length] = 0;
                b.ContentSize = (uint)nm.Length;
                FSBlock.Write(p, b);
                return b;
            }
            return null;
        }
    }
}