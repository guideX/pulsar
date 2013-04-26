// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
using Pulsar.IO;
namespace Pulsar.IO.MemoryMappedFiles {
	[Flags]
	public enum MemoryMappedFileRights {
		CopyOnWrite = 1,
		Write = 2,
		Read  = 4,
		ReadWrite = 6,
		Execute = 8,
		ReadExecute = 12,
		ReadWriteExecute = 14,
		Delete = 0x10000,
		ReadPermissions = 0x20000,
		ChangePermissions = 0x40000,
		TakeOwnership = 0x80000,
		FullControl = 0xf000f,
		AccessSystemSecurity = 0x1000000
	}
}