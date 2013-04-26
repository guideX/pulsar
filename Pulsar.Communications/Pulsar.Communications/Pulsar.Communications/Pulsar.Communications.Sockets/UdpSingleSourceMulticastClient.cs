// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications.Sockets {
	public class UdpSingleSourceMulticastClient : IDisposable {
		const string ObjectDisposed = "UdpSingleSourceMulticastClient instance was disposed.";
		bool disposed;
		public UdpSingleSourceMulticastClient (IPAddress sourceAddress, IPAddress groupAddress, int localPort) {
			if (sourceAddress == null)
				throw new ArgumentNullException ("sourceAddress");
			if (groupAddress == null)
				throw new ArgumentNullException ("groupAddress");
			if ((localPort < 0) || (localPort > 65535))
				throw new ArgumentOutOfRangeException ("localPort");
			if (localPort < 1024)
				throw new SocketException ();

			throw new NotImplementedException ();
		}
		public int ReceiveBufferSize { get; set; }
		public int SendBufferSize { get; set; }

		public IAsyncResult BeginJoinGroup (AsyncCallback callback, object state) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);

			throw new NotImplementedException ();
		}
		public void EndJoinGroup (IAsyncResult result) {
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);

			throw new NotImplementedException ();
		}
		public IAsyncResult BeginReceiveFromSource (byte [] buffer, int offset, int count, AsyncCallback callback, object state) {
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

		public int EndReceiveFromSource (IAsyncResult result, out int sourcePort)
		{
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			if (result == null)
				throw new ArgumentNullException ("result");

			throw new NotImplementedException ();
		}

		public IAsyncResult BeginSendToSource (byte [] buffer, int offset, int count, int remotePort, AsyncCallback callback, object state)
		{
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

		public void EndSendToSource (IAsyncResult result)
		{
			if (disposed)
				throw new ObjectDisposedException (ObjectDisposed);
			if (result == null)
				throw new ArgumentNullException ("result");

			throw new NotImplementedException ();
		}

		public void Dispose ()
		{
			disposed = true;
		}
	}
}
