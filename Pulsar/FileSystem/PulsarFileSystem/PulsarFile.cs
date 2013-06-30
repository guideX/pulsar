using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;

namespace Pulsar.FileSystem.PulsarFileSystem
{
    class PulsarFile : PulsarEntry
    {
        /// <summary>
        /// The FullName (Path+Name) of the Current PulsarFile
        /// </summary>
        public String FullName
        {
            get
            {
                return PulsarFileSystem.CombineDir(_Path, Name);
            }
        }

        /// <summary>
        /// Writes all the specified Bytes into the file
        /// </summary>
        /// <param name="Data">The array Byte to write into file</param>
        public void WriteAllBytes(Byte[] Data)
        {
            if (_StartBlock.NextBlock != 0)
            {
                PulsarFileSystem.ClearBlocks(_StartBlock);
                _StartBlock.NextBlock = 0;
                PulsarFSBlock.Write(PulsarFileSystem.mFS.Partition, _StartBlock);
            }
            int index = 0;
            PulsarFSBlock curb = PulsarFSBlock.GetFreeBlock(PulsarFileSystem.mFS.Partition);
            _StartBlock.NextBlock = curb.BlockNumber;
            PulsarFSBlock.Write(part, _StartBlock);
            do
            {
                Byte[] arr = new Byte[PulsarFSBlock.MaxBlockContentSize];
                index = Utils.CopyByteToByte(Data, index, arr, 0, arr.Length);
                curb.Used = true;
                curb.Content = arr;
                if (index != Data.Length)
                {
                    PulsarFSBlock b = PulsarFSBlock.GetFreeBlock(PulsarFileSystem.mFS.Partition);
                    curb.NextBlock = b.BlockNumber;
                    curb.ContentSize = (uint)arr.Length;
                    PulsarFSBlock.Write(PulsarFileSystem.mFS.Partition, curb);
                    curb = b;
                }
                else
                {
                    curb.ContentSize = (uint)(Data.Length % arr.Length);
                    PulsarFSBlock.Write(PulsarFileSystem.mFS.Partition, curb);
                }
            }
            while (index != Data.Length);
            EditEntryInfo(EntryInfoPosition.DateTimeModified, Environment.DateTime.Now.TimeStamp);
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
        }

        /// <summary>
        /// Writes all the specified text into the file
        /// </summary>
        /// <param name="text">The string to write into file</param>
        public void WriteAllText(String text)
        {
            Byte[] b = new Byte[text.Length];
            Utils.CopyCharToByte(text.ToCharArray(), 0, b, 0, text.Length);
            WriteAllBytes(b);
        }

        /// <summary>
        /// Return's all the bytes contained in the file
        /// </summary>
        public Byte[] ReadAllBytes()
        {
            if (_StartBlock.NextBlock == 0)
            {
                return new Byte[0];
            }
            PulsarFSBlock b = _StartBlock;
            List<Byte> lret = new List<Byte>();
            while (b.NextBlock != 0)
            {
                b = PulsarFSBlock.Read(b.Partition, b.NextBlock);
                for (int i = 0; i < b.ContentSize; i++)
                {
                    lret.Add(b.Content[i]);
                }
            }
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
            return lret.ToArray();
        }

        /// <summary>
        /// Return's all the text contained in the file
        /// </summary>
        public string ReadAllText()
        {
            Byte[] b = ReadAllBytes();
            Char[] text = new Char[b.Length];
            Utils.CopyByteToChar(b, 0, text, 0, b.Length);
            return Utils.CharToString(text);
        }

        /// <summary>
        /// Creates a new PulsarFile Object
        /// </summary>
        /// <param name="p">The partition to use</param>
        /// <param name="bn">The block number we want to use</param>
        /// <param name="pa">The path of the new directory</param>
        public PulsarFile(Partition p, ulong bn,String pa) 
        {
            _Path = pa;
            part = p;
            _StartBlock = PulsarFSBlock.Read(p, bn);
            if (!_StartBlock.Used)
            {
                _StartBlock.Used = true;
                String n = "New File";
                CreateEntry(part, _StartBlock, n);
            }
        }

        /// <summary>
        /// Overrides the ToString Method.
        /// </summary>
        public override String ToString()
        {
            return this.Name;
        }
    }
}
