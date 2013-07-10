using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;
namespace Pulsar.FileSystem.FileSystem {
    class PulsarDirectory : Entry {
        public String FullName {
            get {
                return FileSystem.CombineDir(_Path, Name);
            }
        }
        public PulsarDirectory(Partition p, ulong bn, String pa) {
            _Path = pa;
            part = p;
            _StartBlock = FSBlock .Read(p, bn);
            if(bn == 1 && pa == "/" && _StartBlock.Content[0] != '/') {
                Char[] nm = "/".ToCharArray();
                for(int i = 0; i < nm.Length; i++) {
                    _StartBlock.Content[i] = (byte)nm[i];
                }
                _StartBlock.Used = true;
                _StartBlock.NextBlock = 0;
                FSBlock .Write(p, _StartBlock);
            }
            if(!_StartBlock.Used) {
                _StartBlock.Used = true;
                String n = "New Directory";
                if(pa == FileSystem.separator) {
                    _Path = "";
                    n = pa;
                }
                CreateEntry(part, _StartBlock, n);
            }
        }
        public PulsarDirectory[] GetDirs() {
            FSBlock curb = _StartBlock;
            List<PulsarDirectory> d = new List<PulsarDirectory>();
            while(curb.NextBlock != 0) {
                int index = 0;
                curb = FSBlock .Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(sep == 1) {
                        d.Add(new PulsarDirectory(part, a, FileSystem.CombineDir(_Path, Name)));
                    }
                }
            }
            return d.ToArray();
        }
        public File[] GetFiles() {
            FSBlock curb = _StartBlock;
            List<File> d = new List<File>();
            while(curb.NextBlock != 0) {
                int index = 0;
                curb = FSBlock .Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(sep == 2) {
                        d.Add(new File(part, a, FileSystem.CombineDir(_Path, Name)));
                    }
                }
            }
            return d.ToArray();
        }
        public Entry[] GetEntries() {
            FSBlock curb = _StartBlock;
            List<Entry> d = new List<Entry>();
            while(curb.NextBlock != 0) {
                int index = 0;
                curb = FSBlock .Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(sep == 1) {
                        d.Add(new PulsarDirectory(part, a, FileSystem.CombineDir(_Path, Name)));
                    } else if(sep == 2) {
                        d.Add(new File(part, a, FileSystem.CombineDir(_Path, Name)));
                    }
                }
            }
            return d.ToArray();
        }
        public void AddDirectory(String Name) {
            Entry[] dirs = GetEntries();
            for(int i = 0; i < dirs.Length; i++) {
                if(dirs[i].Name == Name) {
                    Helper.WriteLine("Entry with same Name already exists!");
                    return;
                }
            }
            FSBlock curb = GetBlockToEdit();
            FSBlock newdirb = CreateEntry(part, Name);
            BitConverter.GetBytes(newdirb.BlockNumber).CopyTo(curb.Content, curb.ContentSize);
            BitConverter.GetBytes((uint)1).CopyTo(curb.Content, curb.ContentSize + 8);
            curb.ContentSize += 12;
            FSBlock .Write(part, curb);
            EditEntryInfo(EntryInfoPosition.DateTimeModified, Environment.DateTime.Now.TimeStamp);
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
        }
        public void AddFile(String Name) {
            Entry[] dirs = GetEntries();
            for(int i = 0; i < dirs.Length; i++) {
                if(dirs[i].Name == Name) {
                    Helper.WriteLine("Entry with same Name already exists!");
                    return;
                }
            }
            FSBlock curb = GetBlockToEdit();
            FSBlock newfileb = CreateEntry(part, Name);
            //if (newfileb != null)
            //{
            BitConverter.GetBytes(newfileb.BlockNumber).CopyTo(curb.Content, curb.ContentSize);
            BitConverter.GetBytes((uint)2).CopyTo(curb.Content, curb.ContentSize + 8);
            curb.ContentSize += 12;
            FSBlock .Write(part, curb);
            EditEntryInfo(EntryInfoPosition.DateTimeModified, Environment.DateTime.Now.TimeStamp);
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
        }

        /// <summary>
        /// Permits to remove a PulsarDirectory by passing it's name
        /// </summary>
        /// <param name="Name">The PulsarDirectory's name to remove</param>
        public void RemoveDirectory(String Name) {
            PulsarDirectory[] dirs = GetDirs();
            bool found = false;
            int index = 0;
            for(int i = 0; i < dirs.Length; i++) {
                if(dirs[i].Name == Name) {
                    index = i;
                    found = true;
                    break;
                }
            }
            if(found) {
                RemoveDirectory(dirs[index]);
            }
        }
        private void RemoveDirectory(PulsarDirectory PulsarDirectory) {
            PulsarDirectory[] subdirs = PulsarDirectory.GetDirs();
            for(int i = 0; i < subdirs.Length; i++) {
                PulsarDirectory.RemoveDirectory(subdirs[i]);
            }
            File[] subfiles = PulsarDirectory.GetFiles();
            for(int i = 0; i < subdirs.Length; i++) {
                PulsarDirectory.RemoveFile(subfiles[i].Name);
            }
            FileSystem.ClearBlocks(PulsarDirectory.StartBlock);
            DeleteBlock(PulsarDirectory.StartBlock);
        }

        /// <summary>
        /// Permits to remove a File by passing it's name
        /// </summary>
        /// <param name="Name">The File's name to remove</param>
        public void RemoveFile(String Name) {
            File[] files = GetFiles();
            bool found = false;
            int index = 0;
            for(int i = 0; i < files.Length; i++) {
                if(files[i].Name == Name) {
                    index = i;
                    found = true;
                    break;
                }
            }
            if(found) {
                FileSystem.ClearBlocks(files[index].StartBlock);
                DeleteBlock(files[index].StartBlock);
            }
        }
        private void DeleteBlock(FSBlock FSBlock ) {
            FSBlock curb = _StartBlock;
            while(curb.NextBlock != 0) {
                int index = 0;
                bool found = false;
                List<Byte> cont = new List<Byte>();
                curb = FSBlock .Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    Byte[] app = BitConverter.GetBytes(a);
                    for(int i = 0; i < app.Length; i++) {
                        cont.Add(app[i]);
                    }
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(a == FSBlock .BlockNumber) {
                        app = BitConverter.GetBytes((uint)0);
                        for(int i = 0; i < app.Length; i++) {
                            cont.Add(app[i]);
                        }
                        found = true;
                    } else {
                        app = BitConverter.GetBytes(sep);
                        for(int i = 0; i < app.Length; i++) {
                            cont.Add(app[i]);
                        }
                    }
                }
                if(found) {
                    curb.Content = cont.ToArray();
                    curb.ContentSize = (uint)cont.Count;
                    FSBlock .Write(part, curb);
                }
            }
        }
        private FSBlock GetBlockToEdit() {
            FSBlock ret = _StartBlock;
            while(ret.NextBlock != 0) {
                ret = FSBlock .Read(_StartBlock.Partition, _StartBlock.NextBlock);
            }
            if(ret.BlockNumber == _StartBlock.BlockNumber) {
                ret = FSBlock .GetFreeBlock(part);
                ret.Used = true;
                ret.ContentSize = 0;
                ret.NextBlock = 0;
                _StartBlock.NextBlock = ret.BlockNumber;
                FSBlock .Write(part, _StartBlock);
                FSBlock .Write(part, ret);
            }
            if(part.NewBlockArray(1).Length - ret.ContentSize < 12) {
                FSBlock b = FSBlock .GetFreeBlock(part);
                if(b == null) {
                    return null;
                }
                ret.NextBlock = b.BlockNumber;
                FSBlock .Write(part, ret);
                b.Used = true;
                ret = b;
            }
            return ret;
        }
        public static PulsarDirectory GetDirectoryByFullName(String fn) {
            PulsarDirectory d = FileSystem.mFS.Root;
            if(fn == d.Name) {
                return d;
            }
            if(fn == null || fn == "") {
                return null;
            }
            String[] names = fn.Split('/');
            if(names[0] != "") {
                return null;
            }
            for(int i = 0; i < names.Length; i++) {
                if(names[i] != null && names[i] != "") {
                    d = d.GetDirectoryByName(names[i]);
                    if(d == null) {
                        break;
                    }
                }
            }
            return d;
        }
        public PulsarDirectory GetDirectoryByName(String n) {
            PulsarDirectory[] dirs = GetDirs();
            for(int i = 0; i < dirs.Length; i++) {
                if(dirs[i].Name == n) {
                    return dirs[i];
                }
            }
            return null;
        }
        public override String ToString() {
            return this.Name;
        }
        public static File GetFileByFullName(String fn) {
            PulsarDirectory d = new PulsarDirectory(FileSystem.mFS.Partition, 1, FileSystem.separator);
            if(fn == null || fn == "") {
                return null;
            }
            String[] names = fn.Split('/');
            for(int i = 0; i < names.Length - 1; i++) {
                if(names[i] != "") {
                    d = d.GetDirectoryByName(names[i]);
                    if(d == null) {
                        break;
                    }
                }
            }
            return d.GetFileByName(names[names.Length - 1]);
        }
        public File GetFileByName(String n) {
            File[] files = GetFiles();
            for(int i = 0; i < files.Length; i++) {
                if(files[i].Name == n) {
                    return files[i];
                }
            }
            return null;
        }
    }
}