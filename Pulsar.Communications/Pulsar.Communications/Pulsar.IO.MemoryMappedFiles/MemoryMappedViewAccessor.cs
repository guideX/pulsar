// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
using Pulsar.IO;
//using System.IO;
//using System.Collections.Generic;
//using Microsoft.Win32.SafeHandles;
//using Mono.Unix.Native;
namespace Pulsar.IO.MemoryMappedFiles {
	public sealed class MemoryMappedViewAccessor : UnmanagedMemoryAccessor, IDisposable {
		int file_handle;
		IntPtr mmap_addr;
		SafeMemoryMappedViewHandle handle;

		internal MemoryMappedViewAccessor (int file_handle, long offset, long size, MemoryMappedFileAccess access)
		{
			this.file_handle = file_handle;
			if (MonoUtil.IsUnix)
				CreatePosix (offset, size, access);
			else
				throw new NotImplementedException ("Not implemented on windows.");
		}

		static FileAccess ToFileAccess (MemoryMappedFileAccess access)
		{
			switch (access){
			case MemoryMappedFileAccess.CopyOnWrite:
			case MemoryMappedFileAccess.ReadWrite:
			case MemoryMappedFileAccess.ReadWriteExecute:
				return FileAccess.ReadWrite;
				
			case MemoryMappedFileAccess.Write:
				return FileAccess.Write;
				
			case MemoryMappedFileAccess.ReadExecute:
			case MemoryMappedFileAccess.Read:
			default:
				return FileAccess.Read;
			}
		}
		
		unsafe void CreatePosix (long offset, long size, MemoryMappedFileAccess access)
		{
			int offset_diff;

			MemoryMappedFile.MapPosix (file_handle, offset, ref size, access, out mmap_addr, out offset_diff);

			handle = new SafeMemoryMappedViewHandle ((IntPtr)((long)mmap_addr + offset_diff), size);
			Initialize (handle, 0, size, ToFileAccess (access));
		}

		public SafeMemoryMappedViewHandle SafeMemoryMappedViewHandle {
			get {
				return handle;
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}

		void IDisposable.Dispose () {
			Dispose (true);
		}

		public void Flush ()
		{
			if (MonoUtil.IsUnix)
				Syscall.fsync (file_handle);
			else
				throw new NotImplementedException ("Not implemented on Windows");
		}
	}
}
