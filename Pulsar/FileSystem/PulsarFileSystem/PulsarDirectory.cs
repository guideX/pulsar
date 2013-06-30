using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Hardware.BlockDevice;

namespace Pulsar.FileSystem.PulsarFileSystem {
    class PulsarDirectory : PulsarEntry {
        /// <summary>
        /// The FullName (Path+Name) of the Current PulsarDirectory
        /// </summary>
        public String FullName {
            get {
                return PulsarFileSystem.CombineDir(_Path, Name);
            }
        }

        /// <summary>
        /// Creates a new PulsarDirectory Object
        /// </summary>
        /// <param name="p">The partition to use</param>
        /// <param name="bn">The block number we want to use</param>
        /// <param name="pa">The path of the new directory</param>
        public PulsarDirectory(Partition p, ulong bn, String pa) {
            _Path = pa;
            part = p;
            _StartBlock = PulsarFSBlock.Read(p, bn);
            if(bn == 1 && pa == "/" && _StartBlock.Content[0] != '/') {
                Char[] nm = "/".ToCharArray();
                for(int i = 0; i < nm.Length; i++) {
                    _StartBlock.Content[i] = (byte)nm[i];
                }
                _StartBlock.Used = true;
                _StartBlock.NextBlock = 0;
                PulsarFSBlock.Write(p, _StartBlock);
            }
            if(!_StartBlock.Used) {
                _StartBlock.Used = true;
                String n = "New Directory";
                if(pa == PulsarFileSystem.separator) {
                    _Path = "";
                    n = pa;
                }
                CreateEntry(part, _StartBlock, n);
            }
        }

        /// <summary>
        /// Gets All PulsarDirectories Contained in this one
        /// </summary>
        public PulsarDirectory[] GetDirs() {
            PulsarFSBlock curb = _StartBlock;
            List<PulsarDirectory> d = new List<PulsarDirectory>();
            while(curb.NextBlock != 0) {
                int index = 0;
                curb = PulsarFSBlock.Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(sep == 1) {
                        d.Add(new PulsarDirectory(part, a, PulsarFileSystem.CombineDir(_Path, Name)));
                    }
                }
            }
            return d.ToArray();
        }

        /// <summary>
        /// Gets All PulsarFiles contained in this PulsarDirectory
        /// </summary>
        public PulsarFile[] GetFiles() {
            PulsarFSBlock curb = _StartBlock;
            List<PulsarFile> d = new List<PulsarFile>();
            while(curb.NextBlock != 0) {
                int index = 0;
                curb = PulsarFSBlock.Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(sep == 2) {
                        d.Add(new PulsarFile(part, a, PulsarFileSystem.CombineDir(_Path, Name)));
                    }
                }
            }
            return d.ToArray();
        }

        /// <summary>
        /// Gets all PulsarEntries contained in this PulsarDirectory
        /// </summary>
        public PulsarEntry[] GetEntries() {
            PulsarFSBlock curb = _StartBlock;
            List<PulsarEntry> d = new List<PulsarEntry>();
            while(curb.NextBlock != 0) {
                int index = 0;
                curb = PulsarFSBlock.Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(sep == 1) {
                        d.Add(new PulsarDirectory(part, a, PulsarFileSystem.CombineDir(_Path, Name)));
                    } else if(sep == 2) {
                        d.Add(new PulsarFile(part, a, PulsarFileSystem.CombineDir(_Path, Name)));
                    }
                }
            }
            return d.ToArray();
        }

        /// <summary>
        /// Adds a new PulsarDirectory to the current directory
        /// </summary>
        /// <param name="Name">The new PulsarDirectory's name</param>
        public void AddDirectory(String Name) {
            PulsarEntry[] dirs = GetEntries();
            for(int i = 0; i < dirs.Length; i++) {
                if(dirs[i].Name == Name) {
                    Helper.WriteLine("Entry with same Name already exists!");
                    return;
                }
            }
            PulsarFSBlock curb = GetBlockToEdit();
            PulsarFSBlock newdirb = CreateEntry(part, Name);
            //if (newdirb != null)
            //{
            BitConverter.GetBytes(newdirb.BlockNumber).CopyTo(curb.Content, curb.ContentSize);
            BitConverter.GetBytes((uint)1).CopyTo(curb.Content, curb.ContentSize + 8);
            curb.ContentSize += 12;
            PulsarFSBlock.Write(part, curb);
            EditEntryInfo(EntryInfoPosition.DateTimeModified, Environment.DateTime.Now.TimeStamp);
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
            //}
        }

        /// <summary>
        /// Creates a new PulsarFile to the current directory
        /// </summary>
        /// <param name="Name">The new PulsarFile's name</param>
        public void AddFile(String Name) {
            PulsarEntry[] dirs = GetEntries();
            for(int i = 0; i < dirs.Length; i++) {
                if(dirs[i].Name == Name) {
                    Helper.WriteLine("Entry with same Name already exists!");
                    return;
                }
            }
            PulsarFSBlock curb = GetBlockToEdit();
            PulsarFSBlock newfileb = CreateEntry(part, Name);
            //if (newfileb != null)
            //{
            BitConverter.GetBytes(newfileb.BlockNumber).CopyTo(curb.Content, curb.ContentSize);
            BitConverter.GetBytes((uint)2).CopyTo(curb.Content, curb.ContentSize + 8);
            curb.ContentSize += 12;
            PulsarFSBlock.Write(part, curb);
            EditEntryInfo(EntryInfoPosition.DateTimeModified, Environment.DateTime.Now.TimeStamp);
            EditEntryInfo(EntryInfoPosition.DateTimeLastAccess, Environment.DateTime.Now.TimeStamp);
            //}
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

        /// <summary>
        /// Permits to remove a PulsarDirectory by passing it
        /// </summary>
        /// <param name="PulsarDirectory">The PulsarDirectory to remove</param>
        private void RemoveDirectory(PulsarDirectory PulsarDirectory) {
            PulsarDirectory[] subdirs = PulsarDirectory.GetDirs();
            for(int i = 0; i < subdirs.Length; i++) {
                PulsarDirectory.RemoveDirectory(subdirs[i]);
            }
            PulsarFile[] subfiles = PulsarDirectory.GetFiles();
            for(int i = 0; i < subdirs.Length; i++) {
                PulsarDirectory.RemoveFile(subfiles[i].Name);
            }
            PulsarFileSystem.ClearBlocks(PulsarDirectory.StartBlock);
            DeleteBlock(PulsarDirectory.StartBlock);
        }

        /// <summary>
        /// Permits to remove a PulsarFile by passing it's name
        /// </summary>
        /// <param name="Name">The PulsarFile's name to remove</param>
        public void RemoveFile(String Name) {
            PulsarFile[] files = GetFiles();
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
                PulsarFileSystem.ClearBlocks(files[index].StartBlock);
                DeleteBlock(files[index].StartBlock);
            }
        }

        /// <summary>
        /// Permits to remove a PulsarFSBlock by passing it
        /// </summary>
        /// <param name="PulsarDirectory">The PulsarFSBlock to remove</param>
        private void DeleteBlock(PulsarFSBlock PulsarFSBlock) {
            PulsarFSBlock curb = _StartBlock;
            while(curb.NextBlock != 0) {
                int index = 0;
                bool found = false;
                List<Byte> cont = new List<Byte>();
                curb = PulsarFSBlock.Read(_StartBlock.Partition, _StartBlock.NextBlock);
                while(index < curb.ContentSize) {
                    ulong a = BitConverter.ToUInt64(curb.Content, index);
                    Byte[] app = BitConverter.GetBytes(a);
                    for(int i = 0; i < app.Length; i++) {
                        cont.Add(app[i]);
                    }
                    index += 8;
                    uint sep = BitConverter.ToUInt32(curb.Content, index);
                    index += 4;
                    if(a == PulsarFSBlock.BlockNumber) {
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
                    PulsarFSBlock.Write(part, curb);
                }
            }
        }

        /// <summary>
        /// Gets the last PulsarFSBlock of the directory
        /// </summary>
        private PulsarFSBlock GetBlockToEdit() {
            PulsarFSBlock ret = _StartBlock;
            while(ret.NextBlock != 0) {
                ret = PulsarFSBlock.Read(_StartBlock.Partition, _StartBlock.NextBlock);
            }
            if(ret.BlockNumber == _StartBlock.BlockNumber) {
                ret = PulsarFSBlock.GetFreeBlock(part);
                ret.Used = true;
                ret.ContentSize = 0;
                ret.NextBlock = 0;
                _StartBlock.NextBlock = ret.BlockNumber;
                PulsarFSBlock.Write(part, _StartBlock);
                PulsarFSBlock.Write(part, ret);
            }
            if(part.NewBlockArray(1).Length - ret.ContentSize < 12) {
                PulsarFSBlock b = PulsarFSBlock.GetFreeBlock(part);
                if(b == null) {
                    return null;
                }
                ret.NextBlock = b.BlockNumber;
                PulsarFSBlock.Write(part, ret);
                b.Used = true;
                ret = b;
            }
            return ret;
        }

        /// <summary>
        /// Get the directory specified by the Fullname passed
        /// </summary>
        /// <param name="fn">The fullname of the directory</param>
        public static PulsarDirectory GetDirectoryByFullName(String fn) {
            PulsarDirectory d = PulsarFileSystem.mFS.Root;
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

        /// <summary>
        /// Get the directory specified by the Name passed
        /// </summary>
        /// <param name="n">The name of the child directory</param>
        public PulsarDirectory GetDirectoryByName(String n) {
            PulsarDirectory[] dirs = GetDirs();
            for(int i = 0; i < dirs.Length; i++) {
                if(dirs[i].Name == n) {
                    return dirs[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Overrides the ToString Method.
        /// </summary>
        public override String ToString() {
            return this.Name;
        }

        /// <summary>
        /// Get the directory specified by the Fullname passed
        /// </summary>
        /// <param name="fn">The fullname of the directory</param>
        public static PulsarFile GetFileByFullName(String fn) {
            PulsarDirectory d = new PulsarDirectory(PulsarFileSystem.mFS.Partition, 1, PulsarFileSystem.separator);
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

        /// <summary>
        /// Get the file specified by the Name passed
        /// </summary>
        /// <param name="n">The name of the child file</param>
        public PulsarFile GetFileByName(String n) {
            PulsarFile[] files = GetFiles();
            for(int i = 0; i < files.Length; i++) {
                if(files[i].Name == n) {
                    return files[i];
                }
            }
            return null;
        }
    }
}
