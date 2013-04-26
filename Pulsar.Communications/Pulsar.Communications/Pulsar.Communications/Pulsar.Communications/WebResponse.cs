// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
using Pulsar.IO;
namespace Pulsar.Communications {
	public abstract class WebResponse : IDisposable {
		private WebHeaderCollection headers;
		public abstract long ContentLength { get; }
		public abstract string ContentType { get; }
		public abstract Uri ResponseUri { get; }
		public virtual WebHeaderCollection Headers {
			get {
				if (!SupportsHeaders)
					throw NotImplemented ();
				return headers;
			}
		}
		internal WebHeaderCollection InternalHeaders {
			get { return headers; }
			set { headers = value; }
		}
		public virtual bool SupportsHeaders {
			get { return false; }
		}
		protected WebResponse () {
		}
		public abstract void Close ();
		public abstract Stream GetResponseStream ();

		void IDisposable.Dispose () {
			Close ();
		}
		static Exception NotImplemented () {
			return new NotImplementedException ();
		}
	}
}