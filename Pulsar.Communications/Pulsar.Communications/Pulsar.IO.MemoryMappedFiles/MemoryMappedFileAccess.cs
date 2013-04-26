// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
using Pulsar.IO;
namespace Pulsar.IO.MemoryMappedFiles {
	public enum MemoryMappedFileAccess {
		ReadWrite = 0,
		Read = 1,
		Write = 2,
		CopyOnWrite = 3,
		ReadExecute = 4,
		ReadWriteExecute = 5
	}
}