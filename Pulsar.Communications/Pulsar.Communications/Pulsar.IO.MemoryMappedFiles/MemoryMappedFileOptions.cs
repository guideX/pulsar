// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
using Pulsar;
using Pulsar.IO;
namespace Pulsar.IO.MemoryMappedFiles {
	[Flags]
	public enum MemoryMappedFileOptions {
		None = 0,
		DelayAllocatePages = 0x4000000
	}
}