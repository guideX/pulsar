using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPUAll = Cosmos.Assembler;
using CPUx86 = Cosmos.Assembler.x86;
using Sys = Cosmos.System;
using Cosmos.IL2CPU.Plugs;
namespace Pulsar.Executables
{
        public static class BinaryLoader
        {
            public static uint Address;
            public static void CallRaw(byte[] aData)
            {
                unsafe
                {
                    byte* data = (byte*)Cosmos.Core.Heap.MemAlloc((uint)aData.Length);
                    Address = (uint)&data[0];
                    for (int i = 0; i < aData.Length; i++)
                    {
                        data[i] = aData[i];
                    }   
                    Caller call = new Caller();
                    call.CallCode((uint)&data[0]);
                }
            }
            #region Plug
            public class Caller
            {
                [PlugMethod(Assembler = typeof(CallerPlug))]
                public void CallCode(uint address) { } //Plugged
            }
            [Plug(Target = typeof(Caller))]
            public class CallerPlug : AssemblerMethod
            {
                public override void AssembleNew(object aAssembler, object aMethodInfo)
                {
                    new CPUAll.Comment("Pulsar");
                    new CPUx86.Mov { SourceReg = CPUx86.Registers.EBP, SourceDisplacement = 8, SourceIsIndirect = true, DestinationReg = CPUx86.Registers.EAX };
                    new CPUx86.Call { DestinationReg = CPUx86.Registers.EAX };
                }
            }
            #endregion
        }
}
