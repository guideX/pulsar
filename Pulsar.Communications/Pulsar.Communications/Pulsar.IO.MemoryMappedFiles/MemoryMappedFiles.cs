// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
using Pulsar.IO;
//using System.Collections.Generic;
//using Microsoft.Win32.SafeHandles;
//using Mono.Unix.Native;
//using Mono.Unix;
//using System.Runtime.InteropServices;
namespace Pulsar.IO.MemoryMappedFiles {
	public class MemoryMappedFile : IDisposable {
		MemoryMappedFileAccess fileAccess;
		string name;
		long fileCapacity;
		FileStream stream;
		bool keepOpen;
		int unix_fd;

		public static MemoryMappedFile CreateFromFile (string path)
		{
			return CreateFromFile (path, FileMode.Open, null, 0, MemoryMappedFileAccess.ReadWrite);
		}

		public static MemoryMappedFile CreateFromFile (string path, FileMode mode)
		{
			return CreateFromFile (path, mode, null, 0, MemoryMappedFileAccess.ReadWrite);
		}

		public static MemoryMappedFile CreateFromFile (string path, FileMode mode, string mapName)
		{
			return CreateFromFile (path, mode, mapName, 0, MemoryMappedFileAccess.ReadWrite);
		}

		public static MemoryMappedFile CreateFromFile (string path, FileMode mode, string mapName, long capacity)
		{
			return CreateFromFile (path, mode, mapName, capacity, MemoryMappedFileAccess.ReadWrite);
		}
		static OpenFlags ToUnixMode (FileMode mode)
		{
			switch (mode){
			case FileMode.CreateNew:
				return OpenFlags.O_CREAT | OpenFlags.O_EXCL;
				
			case FileMode.Create:
				return OpenFlags.O_CREAT | OpenFlags.O_TRUNC;
				
			case FileMode.OpenOrCreate:
				return OpenFlags.O_CREAT;
				
			case FileMode.Truncate:
				return OpenFlags.O_TRUNC;
				
			case FileMode.Append:
				return OpenFlags.O_APPEND;
			default:
			case FileMode.Open:
				return 0;
			}
		}
		static OpenFlags ToUnixMode (MemoryMappedFileAccess access)
		{
			switch (access){
			case MemoryMappedFileAccess.CopyOnWrite:
			case MemoryMappedFileAccess.ReadWriteExecute:
			case MemoryMappedFileAccess.ReadWrite:
				return OpenFlags.O_RDWR;
				
			case MemoryMappedFileAccess.Write:
				return OpenFlags.O_WRONLY;

			case MemoryMappedFileAccess.ReadExecute:
			case MemoryMappedFileAccess.Read:
			default:
				return OpenFlags.O_RDONLY;
			}
		}

		public static MemoryMappedFile CreateFromFile (string path, FileMode mode, string mapName, long capacity, MemoryMappedFileAccess access)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			if (path.Length == 0)
				throw new ArgumentException ("path");
			if (mapName != null && mapName.Length == 0)
				throw new ArgumentException ("mapName");

			int fd;
			if (MonoUtil.IsUnix){
				Stat buf;
				if (Syscall.stat (path, out buf) == -1)
					UnixMarshal.ThrowExceptionForLastError ();

				if ((capacity == 0 && buf.st_size == 0) || (capacity > buf.st_size))
					throw new ArgumentException ("capacity");
				
				fd = Syscall.open (path, ToUnixMode (mode) | ToUnixMode (access), FilePermissions.DEFFILEMODE);

				if (fd == -1)
					UnixMarshal.ThrowExceptionForLastError ();
			} else
				throw new NotImplementedException ();
			
			return new MemoryMappedFile () {
				unix_fd = fd,
				fileAccess = access,
				name = mapName,
				fileCapacity = capacity
			};
		}

		static void ConfigureUnixFD (IntPtr handle, HandleInheritability h)
		{
			// TODO: Mono.Posix is lacking O_CLOEXEC definitions for fcntl.
		}


		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetHandleInformation (IntPtr hObject, int dwMask, int dwFlags);
		static void ConfigureWindowsFD (IntPtr handle, HandleInheritability h)
		{
			SetHandleInformation (handle, 1 /* FLAG_INHERIT */, h == HandleInheritability.None ? 0 : 1);
		}

		[MonoLimitation ("memoryMappedFileSecurity is currently ignored")]
		public static MemoryMappedFile CreateFromFile (FileStream fileStream, string mapName, long capacity, MemoryMappedFileAccess access,
							       MemoryMappedFileSecurity memoryMappedFileSecurity, HandleInheritability inheritability,
							       bool leaveOpen)
		{
			if (fileStream == null)
				throw new ArgumentNullException ("fileStream");
			if (mapName != null && mapName.Length == 0)
				throw new ArgumentException ("mapName");
			if ((capacity == 0 && fileStream.Length == 0) || (capacity > fileStream.Length))
				throw new ArgumentException ("capacity");

			if (MonoUtil.IsUnix)
				ConfigureUnixFD (fileStream.Handle, inheritability);
			else
				ConfigureWindowsFD (fileStream.Handle, inheritability);
				
			return new MemoryMappedFile () {
				stream = fileStream,
				fileAccess = access,
				name = mapName,
				fileCapacity = capacity,
				keepOpen = leaveOpen
			};
		}

		[MonoLimitation ("CreateNew requires that mapName be a file name on Unix")]
		public static MemoryMappedFile CreateNew (string mapName, long capacity)
		{
			return CreateNew (mapName, capacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.DelayAllocatePages, null, 0);
		}

		[MonoLimitation ("CreateNew requires that mapName be a file name on Unix")]
		public static MemoryMappedFile CreateNew (string mapName, long capacity, MemoryMappedFileAccess access) 
		{
			return CreateNew (mapName, capacity, access, MemoryMappedFileOptions.DelayAllocatePages, null, 0);
		}

		public static MemoryMappedFile CreateNew (string mapName, long capacity, MemoryMappedFileAccess access,
							  MemoryMappedFileOptions options, MemoryMappedFileSecurity memoryMappedFileSecurity,
							  HandleInheritability handleInheritability)
		{
			return CreateFromFile (mapName, FileMode.CreateNew, mapName, capacity, access);
		}

		public static MemoryMappedFile CreateOrOpen (string mapName, long capacity)
		{
			return CreateOrOpen (mapName, capacity, MemoryMappedFileAccess.ReadWrite);
		}

		public static MemoryMappedFile CreateOrOpen (string mapName, long capacity, MemoryMappedFileAccess access)
		{
			return CreateFromFile (mapName, FileMode.OpenOrCreate, mapName, capacity, access); 
		}

		public static MemoryMappedFile CreateOrOpen (string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, MemoryMappedFileSecurity memoryMappedFileSecurity, HandleInheritability handleInheritability)
		{
			throw new NotImplementedException ();
		}

		public static MemoryMappedFile OpenExisting (string mapName)
		{
			throw new NotImplementedException ();
		}

		public static MemoryMappedFile OpenExisting (string mapName, MemoryMappedFileRights desiredAccessRights)
		{
			throw new NotImplementedException ();
		}

		public static MemoryMappedFile OpenExisting (string mapName, MemoryMappedFileRights desiredAccessRights, HandleInheritability inheritability)
		{
			throw new NotImplementedException ();
		}

		public MemoryMappedViewStream CreateViewStream ()
		{
			return CreateViewStream (0, 0);
		}

		public MemoryMappedViewStream CreateViewStream (long offset, long size)
		{
			return CreateViewStream (offset, size, MemoryMappedFileAccess.ReadWrite);
		}

		public MemoryMappedViewStream CreateViewStream (long offset, long size, MemoryMappedFileAccess access)
		{
			return new MemoryMappedViewStream (stream != null ? (int)stream.Handle : unix_fd, offset, size, access);
		}

		public MemoryMappedViewAccessor CreateViewAccessor ()
		{
			return CreateViewAccessor (0, 0);
		}

		public MemoryMappedViewAccessor CreateViewAccessor (long offset, long size)
		{
			return CreateViewAccessor (offset, size, MemoryMappedFileAccess.ReadWrite);
		}

		public MemoryMappedViewAccessor CreateViewAccessor (long offset, long size, MemoryMappedFileAccess access)
		{
			int file_handle = stream != null ? (int) stream.Handle : unix_fd;
			
			return new MemoryMappedViewAccessor (file_handle, offset, size, access);
		}

		MemoryMappedFile ()
		{
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing){
				if (stream != null){
					if (keepOpen == false)
						stream.Close ();
					unix_fd = -1;
				}
				if (unix_fd != -1)
					Syscall.close (unix_fd);
				unix_fd = -1;
				stream = null;
			}
		}

		public MemoryMappedFileSecurity GetAccessControl ()
		{
			throw new NotImplementedException ();
		}

		public void SetAccessControl (MemoryMappedFileSecurity memoryMappedFileSecurity)
		{
			throw new NotImplementedException ();
		}

		public SafeMemoryMappedFileHandle SafeMemoryMappedFileHandle {
			get {
				throw new NotImplementedException ();
			}
		}

		static int pagesize;

		static MmapProts ToUnixProts (MemoryMappedFileAccess access)
		{
			switch (access){
			case MemoryMappedFileAccess.ReadWrite:
				return MmapProts.PROT_WRITE | MmapProts.PROT_READ;
				
			case MemoryMappedFileAccess.Write:
				return MmapProts.PROT_WRITE;
				
			case MemoryMappedFileAccess.CopyOnWrite:
				return MmapProts.PROT_WRITE | MmapProts.PROT_READ;
				
			case MemoryMappedFileAccess.ReadExecute:
				return MmapProts.PROT_EXEC;
				
			case MemoryMappedFileAccess.ReadWriteExecute:
				return MmapProts.PROT_WRITE | MmapProts.PROT_READ | MmapProts.PROT_EXEC;
				
			case MemoryMappedFileAccess.Read:
			default:
				return MmapProts.PROT_READ;
			}
		}

		internal static unsafe void MapPosix (int file_handle, long offset, ref long size, MemoryMappedFileAccess access, out IntPtr map_addr, out int offset_diff)
		{
			if (pagesize == 0)
				pagesize = Syscall.getpagesize ();

			Stat buf;
			Syscall.fstat (file_handle, out buf);
			long fsize = buf.st_size;

			if (size == 0 || size > fsize)
				size = fsize;
			long real_offset = offset & ~(pagesize - 1);
			offset_diff = (int)(offset - real_offset);
			map_addr = Syscall.mmap (IntPtr.Zero, (ulong) size,
						 ToUnixProts (access),
						 access == MemoryMappedFileAccess.CopyOnWrite ? MmapFlags.MAP_PRIVATE : MmapFlags.MAP_SHARED,
						 file_handle, real_offset);

			if (map_addr == (IntPtr)(-1))
				throw new IOException ("mmap failed for fd#" + file_handle + "(" + offset + ", " + size + ")");
		}

		internal static bool UnmapPosix (IntPtr map_addr, ulong map_size)
		{
			return Syscall.munmap (map_addr, map_size) == 0;
		}

	}
}