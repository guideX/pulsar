using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;
using System.ComponentModel;
using Pulsar;

namespace Pulsar.FileSystem.PulsarFileSystem
{
    enum EntryInfoPosition
    {
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
        //Space for Something else 
    }

    class PulsarEntry
    {
        protected static Char[] InvalidChars = new Char[] { '/', '?', '*' };

        protected PulsarFSBlock _StartBlock;
        protected Partition part;
        protected String _Path;

        private static int MaxNameSize = 255;

        private int CustomSize = (int)(PulsarFSBlock.MaxBlockContentSize - MaxNameSize);

        /// <summary>
        /// The StartBlock of the current PulsarDirectory
        /// </summary>
        public PulsarFSBlock StartBlock
        {
            get
            {
                return _StartBlock;
            }
        }

        /// <summary>
        /// The Path of the Current PulsarDirectory
        /// </summary>
        public String Path
        {
            get
            {
                return _Path;
            }
        }

        /// <summary>
        /// The Name of the Current PulsarDirectory
        /// </summary>
        public String Name
        {
            get
            {
                Byte[] arr = _StartBlock.Content;
                String str = "";
                for (int i = 0; i < MaxNameSize; i++)
                {
                    if (arr[i] == 0)
                        break;
                    str += ((Char)arr[i]).ToString();
                }
                return str;
            }
        }

        /// <summary>
        /// Permits to edit an entryInfo
        /// </summary>
        /// <param name="pos">The EntryInfo to edit</param>
        /// <param name="value">The new value</param>
        public void EditEntryInfo(EntryInfoPosition pos, long value)
        {
            if (pos < EntryInfoPosition.Visible)
            {
                Utils.CopyByteToByte(BitConverter.GetBytes(value), 0, _StartBlock.Content, (int)MaxNameSize + (int)pos, 8, false);
            }
            else
            {
                Utils.CopyByteToByte(BitConverter.GetBytes(value), 0, _StartBlock.Content, (int)MaxNameSize + (int)pos, 1, false);
            }
        }

        /// <summary>
        /// Creates a new PulsarEntry in the current directory. Uses a random Block.
        /// </summary>
        /// <param name="p">The partition where to create the new PulsarEntry</param>
        /// <param name="name">The new PulsarEntry's name</param>
        protected static PulsarFSBlock CreateEntry(Partition p, String name)
        {
            return CreateEntry(p, PulsarFSBlock.GetFreeBlock(p), name);
        }

        /// <summary>
        /// Creates a new PulsarEntry in the current directory
        /// </summary>
        /// <param name="p">The partition where to create the new PulsarEntry</param>
        /// <param name="b">The block to write on</param>
        /// <param name="name">The new PulsarEntry's name</param>
        protected static PulsarFSBlock CreateEntry(Partition p, PulsarFSBlock b, String n)
        {
            if (b != null && ((!Utils.StringContains(n, InvalidChars)) || b.BlockNumber == 0))
            {
                b.Used = true;
                b.NextBlock = 0;
                b.TotalSize = 0;
                Char[] nm = n.ToCharArray();
                for (int i = 0; i < nm.Length; i++)
                {
                    b.Content[i] = (byte)nm[i];
                }
                if (b.BlockNumber != 0)
                {
                    Utils.CopyByteToByte(BitConverter.GetBytes(Environment.DateTime.Now.TimeStamp), 0, b.Content, (int)MaxNameSize + (int)EntryInfoPosition.DateTimeCreated, 8, false);
                }
                b.Content[nm.Length] = 0;
                b.ContentSize = (uint)nm.Length;
                PulsarFSBlock.Write(p, b);
                return b;
            }
            return null;
        }
    }
}
