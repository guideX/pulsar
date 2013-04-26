//using System;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Security.AccessControl;
//using System.Security.Permissions;
//using System.Security.Principal;
//using Microsoft.Win32.SafeHandles;
using Pulsar.IO;
namespace System.IO.Pipes {
  [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
  public sealed class AnonymousPipeClientStream : PipeStream {
    static SafePipeHandle ToSafePipeHandle(string pipeHandleAsString) {
      if (pipeHandleAsString == null)
        throw new ArgumentNullException("pipeHandleAsString");
      return new SafePipeHandle(new IntPtr(long.Parse(pipeHandleAsString, NumberFormatInfo.InvariantInfo)), false);
    }
    public AnonymousPipeClientStream(string pipeHandleAsString)
      : this(PipeDirection.In, pipeHandleAsString) {
    }
    public AnonymousPipeClientStream(PipeDirection direction, string pipeHandleAsString)
      : this(direction, ToSafePipeHandle(pipeHandleAsString)) {
    }
    public AnonymousPipeClientStream(PipeDirection direction, SafePipeHandle safePipeHandle)
      : base(direction, DefaultBufferSize) {
      InitializeHandle(safePipeHandle, false, false);
      IsConnected = true;
    }

    public override PipeTransmissionMode ReadMode {
      set {
        if (value == PipeTransmissionMode.Message)
          throw new NotSupportedException();
      }
    }

    public override PipeTransmissionMode TransmissionMode {
      get { return PipeTransmissionMode.Byte; }
    }
  }
}