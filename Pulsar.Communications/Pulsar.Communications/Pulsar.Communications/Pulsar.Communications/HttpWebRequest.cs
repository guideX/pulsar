// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
using Pulsar.IO;
namespace Pulsar.Communications {
	public abstract class HttpWebRequest : WebRequest {
		private WebHeaderCollection headers;
		protected HttpWebRequest () {
		}

		public string Accept {
			get { return Headers [HttpRequestHeader.Accept]; }
			set { Headers.SetHeader ("accept", value); }
		}

		public virtual bool AllowReadStreamBuffering {
			get { throw NotImplemented (); }
			set { throw NotImplemented (); }
		}

		// new in SL4 RC
		public virtual bool AllowWriteStreamBuffering {
			get { throw NotImplemented (); }
			set { throw NotImplemented (); }
		}

		public override string ContentType {
			get { return Headers [HttpRequestHeader.ContentType]; }
			set { Headers.SetHeader ("content-type", value); }
		}

		public virtual bool HaveResponse {
			get { throw NotImplemented (); }
		}

		public override WebHeaderCollection Headers {
			get {
				if (headers == null)
					headers = new WebHeaderCollection (true);
				return headers;
			}
			set {
				foreach (string header in value) {
					WebHeaderCollection.ValidateHeader (header);
				}
				Headers.Clear ();
				foreach (string header in value) {
					headers [header] = value [header];
				}
			}
		}

		public virtual CookieContainer CookieContainer {
			get { throw NotImplemented (); }
			set { throw NotImplemented (); }
		}

		public override string Method {
			get { throw NotImplemented (); }
			set { throw NotImplemented (); }
		}

		public override Uri RequestUri {
			get { throw NotImplemented (); }
		}

		// new in SL4 RC
		public virtual bool SupportsCookieContainer {
			get { return false; }
		}

		public override void Abort ()
		{
			throw NotImplemented ();
		}

		public override IAsyncResult BeginGetRequestStream (AsyncCallback callback, object state)
		{
			throw NotImplemented ();
		}

		public override IAsyncResult BeginGetResponse (AsyncCallback callback, object state)
		{
			throw NotImplemented ();
		}

		public override Stream EndGetRequestStream (IAsyncResult asyncResult)
		{
			throw NotImplemented ();
		}

		public override WebResponse EndGetResponse (IAsyncResult asyncResult)
		{
			throw NotImplemented ();
		}

		static Exception NotImplemented ()
		{
			// a bit less IL and hide the "normal" NotImplementedException from corcompare-like tools
			return new NotImplementedException ();
		}
	}
}