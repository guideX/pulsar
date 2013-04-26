// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications { 
	public abstract class HttpWebResponse : WebResponse {
		public virtual string Method {
			get { throw NotImplemented (); }
		}

		public virtual HttpStatusCode StatusCode {
			get { throw NotImplemented (); }
		}

		public virtual string StatusDescription {
			get { throw NotImplemented (); }
		}

		public virtual CookieCollection Cookies { 
			get { throw NotImplemented (); }
		}

		static Exception NotImplemented ()
		{
			// hide the "normal" NotImplementedException from corcompare-like tools
			return new NotImplementedException ();
		}
	}
}