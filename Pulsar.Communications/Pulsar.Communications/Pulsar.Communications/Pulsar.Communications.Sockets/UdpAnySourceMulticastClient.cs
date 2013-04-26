// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications.Sockets {
	public class UdpAnySourceMulticastClient : IDisposable {
		const string ObjectDisposed = "UdpAnySourceMulticastClient instance was disposed.";
		bool disposed;
		public UdpAnySourceMulticastClient (IPAddress groupAddress, int localPort) {
			if (groupAddress == null)
				throw new ArgumentNullException ("groupAddress");
			if ((localPort < 0) || (localPort > 65535))
				throw new ArgumentOutOfRangeException ("localPort");
			if (localPort < 1024)
				throw new SocketException ();
			throw new NotImplementedException ();
		}
		public bool MulticastLoopback { get; set; }
		public int ReceiveBufferSize { get; set; }
		public int SendBufferSize { get; set; }
		public IAsyncResult BeginJoinGroup (AsyncCallback callback, object state) {
      if (disposed) {
        throw new ObjectDisposedException(ObjectDisposed);
        throw new NotImplementedException();
      }
		}
		public void EndJoinGroup (IAsyncResult result) {
			if (result == null)
				throw new ArgumentNullException ("result");
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			throw new NotImplementedException ();
		}
		public IAsyncResult BeginReceiveFromGroup (byte [] buffer, int offset, int count, AsyncCallback callback, object state) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			if ((offset < 0) || (offset > buffer.Length))
				throw new ArgumentOutOfRangeException ("offset");
			if ((count < 0) || (count > buffer.Length - offset))
				throw new ArgumentOutOfRangeException ("count");
			throw new NotImplementedException ();
		}
		public int EndReceiveFromGroup (IAsyncResult result, out IPEndPoint source) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			throw new NotImplementedException ();
		}
		public IAsyncResult BeginSendTo (byte [] buffer, int offset, int count, IPEndPoint remoteEndPoint, AsyncCallback callback, object state) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			if ((offset < 0) || (offset > buffer.Length))
				throw new ArgumentOutOfRangeException ("offset");
			if ((count < 0) || (count > buffer.Length - offset))
				throw new ArgumentOutOfRangeException ("count");

			throw new NotImplementedException ();
		}
		public void EndSendTo (IAsyncResult result) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			if (result == null)
				throw new ArgumentNullException ("result");

			throw new NotImplementedException ();
		}
		public IAsyncResult BeginSendToGroup (byte [] buffer, int offset, int count, AsyncCallback callback, object state) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			if ((offset < 0) || (offset > buffer.Length))
				throw new ArgumentOutOfRangeException ("offset");
			if ((count < 0) || (count > buffer.Length - offset))
				throw new ArgumentOutOfRangeException ("count");
			throw new NotImplementedException ();
		}
		public void EndSendToGroup (IAsyncResult result) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			if (result == null)
				throw new ArgumentNullException ("result");

			throw new NotImplementedException ();
		}
		public void BlockSource (IPAddress sourceAddress) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);

			throw new NotImplementedException ();
		}

		public void UnblockSource (IPAddress sourceAddress)
		{
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);

			throw new NotImplementedException ();
		}
		public void Dispose (){
			disposed = true;
		}
	}
}