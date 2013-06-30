using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;
namespace Pulsar.FileSystem.FileSystem {
    class FSBlock {
        public static uint MaxBlockContentSize = 491;
        private Partition _Partition;
        private ulong _BlockNumber = 0;
        private bool _Used = false;
        private uint _ContentSize = 0;
        private ulong _TotalSize = 0;
        private ulong _NextBlock = 0;
        public Partition Partition {
            get {
                return _Partition;
            }
        }
        public ulong BlockNumber {
            get {
                return _BlockNumber;
            }
        }
        public bool Used {
            get {
                return _Used;
            }
            set {
                _Used = value;
            }
        }
        public uint ContentSize {
            get {
                return _ContentSize;
            }
            set {
                _ContentSize = value;
            }
        }
        public ulong TotalSize {
            get {
                return _TotalSize;
            }
            set {
                _TotalSize = value;
            }
        }
        public ulong NextBlock {
            get {
                return _NextBlock;
            }
            set {
                _NextBlock = value;
            }
        }
        public Byte[] Content;
        public FSBlock (Byte[] Data, Partition p, ulong bn) {
            _BlockNumber = bn;
            _Partition = p;
            Content = new Byte[Data.Length - 21];
            if(Data[0] == 0x00) {
                _Used = false;
                for(int i = 0; i < Content.Length; i++) {
                    Content[i] = 0;
                }
            } else {
                _Used = true;
                _ContentSize = BitConverter.ToUInt32(Data, 1);
                _TotalSize = BitConverter.ToUInt64(Data, 5);
                _NextBlock = BitConverter.ToUInt64(Data, 13);
                for(int i = 21; i < Data.Length; i++) {
                    Content[i - 21] = Data[i];
                }
            }
        }
        public static FSBlock Read(Partition p, ulong bn) {
            Byte[] data = p.NewBlockArray(1);
            p.ReadBlock(bn, 1, data);
            return new FSBlock (data, p, bn);
        }
        public static void Write(Partition p, FSBlock b) {
            Byte[] data = new Byte[p.BlockSize];
            int index = 0;
            if(b.Used) {
                data[index++] = 0x01;
            } else {
                data[index++] = 0x00;
            }
            Byte[] x = BitConverter.GetBytes(b.ContentSize);
            for(int i = 0; i < x.Length; i++) {
                data[index++] = x[i];
            }
            x = BitConverter.GetBytes(b.TotalSize);
            for(int i = 0; i < x.Length; i++) {
                data[index++] = x[i];
            }
            x = BitConverter.GetBytes(b.NextBlock);
            for(int i = 0; i < x.Length; i++) {
                data[index++] = x[i];
            }
            x = b.Content;
            for(int i = 0; i < x.Length; i++) {
                data[index++] = x[i];
            }
            p.WriteBlock(b.BlockNumber, 1, data);
        }
        public static FSBlock GetFreeBlock(Partition p) {
            for(ulong i = 1; i < p.BlockCount; i++) {
                FSBlock b = Read(p, i);
                if(!b.Used) {
                    return b;
                }
            }
            return null;
        }
    }
}