using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsar.Socket {
    internal class AcceptAsyncResult : ContextAwareResult

    [SuppressUnmanagedCodeSecurity]
    internal delegate bool AcceptExDelegate(SafeCloseSocket listenSocketHandle, SafeCloseSocket acceptSocketHandle, IntPtr buffer, int len, int localAddressLength, int remoteAddressLength, out int bytesReceived, IntPtr overlapped)

    internal class AcceptOverlappedAsyncResult : BaseOverlappedAsyncResult

    public enum AddressFamily

    [Flags]
    internal enum AsyncEventBits

    internal enum AsyncEventBitsPos

    internal class BaseOverlappedAsyncResult : ContextAwareResult

    internal class ConnectAsyncResult : ContextAwareResult

    [SuppressUnmanagedCodeSecurity]
    internal delegate bool ConnectExDelegate(SafeCloseSocket socketHandle, IntPtr socketAddress, int socketAddressSize, IntPtr buffer, int dataLength, out int bytesSent, IntPtr overlapped)

    internal class ConnectOverlappedAsyncResult : BaseOverlappedAsyncResult

    [SuppressUnmanagedCodeSecurity]
    internal delegate bool DisconnectExDelegate(SafeCloseSocket socketHandle, IntPtr overlapped, int flags, int reserved)

    [SuppressUnmanagedCodeSecurity]
    internal delegate bool DisconnectExDelegate_Blocking(IntPtr socketHandle, IntPtr overlapped, int flags, int reserved)

    internal class DisconnectOverlappedAsyncResult : BaseOverlappedAsyncResult

    internal sealed class DynamicWinsockMethods

    [SuppressUnmanagedCodeSecurity]
    internal delegate void GetAcceptExSockaddrsDelegate(IntPtr buffer, int receiveDataLength, int localAddressLength, int remoteAddressLength, out IntPtr localSocketAddress, out int localSocketAddressLength, out IntPtr remoteSocketAddress, out int remoteSocketAddressLength)

    public enum IOControlCode

    internal static class IoctlSocketConstants

    [StructLayout(LayoutKind.Sequential)]
    public struct IPPacketInformation

    public enum IPProtectionLevel

    public class IPv6MulticastOption

    public class LingerOption

    public class MulticastOption

    internal abstract class MultipleConnectAsync

    internal class MultipleSocketMultipleConnectAsync : MultipleConnectAsync

    [StructLayout(LayoutKind.Sequential)]
    internal struct NetworkEvents

    public class NetworkStream : Stream

    internal class OverlappedAsyncResult : BaseOverlappedAsyncResult

    internal class OverlappedCache

    public enum ProtocolFamily

    public enum ProtocolType

    internal class ReceiveMessageOverlappedAsyncResult : BaseOverlappedAsyncResult

    public enum SelectMode

    public class SendPacketsElement

    internal class SingleSocketMultipleConnectAsync : MultipleConnectAsync

    public class Socket : IDisposable

    public class SocketAsyncEventArgs : EventArgs, IDisposable

    public enum SocketAsyncOperation

    public enum SocketError

    [Serializable]
    public class SocketException : Win32Exception

    [Flags]
    public enum SocketFlags

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SocketInformation

    [Flags]
    public enum SocketInformationOptions

    public enum SocketOptionLevel

    public enum SocketOptionName

    public enum SocketShutdown

    public enum SocketType

    public class TcpClient : IDisposable

    public class TcpListener

    [StructLayout(LayoutKind.Sequential)]
    internal struct TimeValue

    [Flags]
    public enum TransmitFileOptions

    internal class TransmitFileOverlappedAsyncResult : BaseOverlappedAsyncResult

    [SuppressUnmanagedCodeSecurity]
    internal delegate bool TransmitPacketsDelegate(SafeCloseSocket socketHandle, IntPtr packetArray, int elementCount, int sendSize, IntPtr overlapped, TransmitFileOptions flags)

    public class UdpClient : IDisposable

    [SuppressUnmanagedCodeSecurity]
    internal delegate SocketError WSARecvMsgDelegate(SafeCloseSocket socketHandle, IntPtr msg, out int bytesTransferred, IntPtr overlapped, IntPtr completionRoutine)

    [SuppressUnmanagedCodeSecurity]
    internal delegate SocketError WSARecvMsgDelegate_Blocking(IntPtr socketHandle, IntPtr msg, out int bytesTransferred, IntPtr overlapped, IntPtr completionRoutine
    }
