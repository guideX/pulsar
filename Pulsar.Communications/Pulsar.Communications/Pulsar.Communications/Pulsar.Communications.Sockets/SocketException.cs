// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System.Runtime.CompilerServices;
namespace Pulsar.Communications.Sockets {
  public class SocketException : Exception {
    int error_code;
    [MethodImplAttribute(MethodImplOptions.InternalCall)]
    private static extern int WSAGetLastError_internal();

    public SocketException() {
      error_code = WSAGetLastError_internal();
    }

    public SocketException(int error) {
      error_code = error;
    }

    internal SocketException(int error, string message)
      : base(message) {
      error_code = error;
    }

    public int ErrorCode {
      get { return error_code; }
    }

    public SocketError SocketErrorCode {
      get { return (SocketError)error_code; }
    }
  }
}