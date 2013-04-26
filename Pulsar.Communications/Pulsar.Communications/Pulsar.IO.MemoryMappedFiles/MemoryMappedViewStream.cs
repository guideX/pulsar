// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
//using System.IO;
//using Mono.Unix.Native;
using Pulsar.IO;
namespace System.IO.MemoryMappedFiles
{
	public sealed class MemoryMappedViewStream : UnmanagedMemoryStream {
		IntPtr mmap_addr;
		ulong mmap_size;
		object monitor;
		int fd;
		
		internal MemoryMappedViewStream (int fd, long offset, long size, MemoryMappedFileAccess access) {
			this.fd = fd;
			monitor = new Object ();
			if (MonoUtil.IsUnix)
				CreateStreamPosix (fd, offset, size, access);
			else
				throw new NotImplementedException ("Not implemented on windows.");
		}

		unsafe void CreateStreamPosix (int fd, long offset, long size, MemoryMappedFileAccess access)
		{
			int offset_diff;
			mmap_size = (ulong) size;
			MemoryMappedFile.MapPosix (fd, offset, ref size, access, out mmap_addr, out offset_diff);
			FileAccess faccess;

			switch (access) {
			case MemoryMappedFileAccess.ReadWrite:
				faccess = FileAccess.ReadWrite;
				break;
			case MemoryMappedFileAccess.Read:
				faccess = FileAccess.Read;
				break;
			case MemoryMappedFileAccess.Write:
				faccess = FileAccess.Write;
				break;
			default:
				throw new NotImplementedException ("access mode " + access + " not supported.");
			}
			Initialize ((byte*)mmap_addr + offset_diff, size, size, faccess);
		}
		 
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			lock (monitor) {
				if (mmap_addr != (IntPtr)(-1)) {
					MemoryMappedFile.UnmapPosix (mmap_addr, mmap_size);
					mmap_addr = (IntPtr)(-1);
				}
			}
		}

		public override void Flush ()
		{
			if (MonoUtil.IsUnix)
				Syscall.fsync (fd);
			else
				throw new NotImplementedException ("Not implemented on Windows");
		}
	}
}