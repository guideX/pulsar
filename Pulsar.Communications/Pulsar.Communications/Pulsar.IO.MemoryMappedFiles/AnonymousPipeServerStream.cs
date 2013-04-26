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
  public sealed class AnonymousPipeServerStream : PipeStream {
    public AnonymousPipeServerStream()
      : this(PipeDirection.Out) {
    }
    public AnonymousPipeServerStream(PipeDirection direction)
      : this(direction, HandleInheritability.None) {
    }
    public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability)
      : this(direction, inheritability, DefaultBufferSize) {
    }
    public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize)
      : this(direction, inheritability, bufferSize, null) {
    }
    public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity pipeSecurity)
      : base(direction, bufferSize) {
      if (pipeSecurity != null)
        throw ThrowACLException();
      if (direction == PipeDirection.InOut)
        throw new NotSupportedException("Anonymous pipe direction can only be either in or out.");
      if (IsWindows)
        impl = new Win32AnonymousPipeServer(this, direction, inheritability, bufferSize);
      else
        impl = new UnixAnonymousPipeServer(this, direction, inheritability, bufferSize);
      InitializeHandle(impl.Handle, false, false);
      IsConnected = true;
    }
    public AnonymousPipeServerStream(PipeDirection direction, SafePipeHandle serverSafePipeHandle, SafePipeHandle clientSafePipeHandle)
      : base(direction, DefaultBufferSize) {
      if (serverSafePipeHandle == null)
        throw new ArgumentNullException("serverSafePipeHandle");
      if (clientSafePipeHandle == null)
        throw new ArgumentNullException("clientSafePipeHandle");
      if (direction == PipeDirection.InOut)
        throw new NotSupportedException("Anonymous pipe direction can only be either in or out.");
      if (IsWindows)
        impl = new Win32AnonymousPipeServer(this, serverSafePipeHandle, clientSafePipeHandle);
      else
        impl = new UnixAnonymousPipeServer(this, serverSafePipeHandle, clientSafePipeHandle);
      InitializeHandle(serverSafePipeHandle, true, false);
      IsConnected = true;
      ClientSafePipeHandle = clientSafePipeHandle;
    }
    IAnonymousPipeServer impl;
    public SafePipeHandle ClientSafePipeHandle { get; private set; }
    public override PipeTransmissionMode ReadMode {
      set {
        if (value == PipeTransmissionMode.Message)
          throw new NotSupportedException();
      }
    }
    public override PipeTransmissionMode TransmissionMode {
      get { return PipeTransmissionMode.Byte; }
    }
    public void DisposeLocalCopyOfClientHandle() {
      impl.DisposeLocalCopyOfClientHandle();
    }
    public string GetClientHandleAsString() {
      return impl.Handle.DangerousGetHandle().ToInt64().ToString(NumberFormatInfo.InvariantInfo);
    }
  }
}
