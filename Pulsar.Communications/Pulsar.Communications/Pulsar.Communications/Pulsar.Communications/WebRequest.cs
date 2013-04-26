// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System.Collections.Generic;
using Pulsar.IO;
namespace Pulsar.Communications {
	public abstract class WebRequest {
		const string SystemWindows = "System.Windows, PublicKey=00240000048000009400000006020000002400005253413100040000010001008D56C76F9E8649383049F383C44BE0EC204181822A6C31CF5EB7EF486944D032188EA1D3920763712CCB12D75FB77E9811149E6148E5D32FBAAB37611C1878DDC19E20EF135D0CB2CFF2BFEC3D115810C3D9069638FE4BE215DBF795861920E5AB6F7DB2E2CEEF136AC23D5DD2BF031700AEC232F6C6B1C785B4305C123B37AB";
		const string BrowserStack = "System.Net.Browser.BrowserHttpWebRequestCreator, " + SystemWindows;
		const string ClientStack = "System.Net.Browser.ClientHttpWebRequestCreator, " + SystemWindows;
		static IWebRequestCreate default_creator;
		static IWebRequestCreate browser_creator;
		static IWebRequestCreate client_creator;
		static Dictionary<string, IWebRequestCreate> registred_prefixes;
		internal Action<long,long> progress;
		public abstract string ContentType { get; set; }
		public abstract WebHeaderCollection Headers { get; set; }
		public abstract string Method { get; set; }
		public abstract Uri RequestUri { get; }
		public virtual long ContentLength {
			get { throw NotImplemented (); }
			set { throw NotImplemented (); }
		}
		public virtual IWebRequestCreate CreatorInstance { 
			get { return null; }
		}
		public virtual ICredentials Credentials {
			get { throw NotImplemented (); }
			set { throw NotImplemented (); }
		}
		public virtual bool UseDefaultCredentials {
			get { throw NotImplemented (); }
			set { throw NotImplemented (); }
		}
		static WebRequest () {
			registred_prefixes = new Dictionary<string, IWebRequestCreate> (StringComparer.OrdinalIgnoreCase);
			browser_creator = (IWebRequestCreate) Activator.CreateInstance (Type.GetType (BrowserStack));
			client_creator = (IWebRequestCreate) Activator.CreateInstance (Type.GetType (ClientStack));
			default_creator = browser_creator;
		}
		protected WebRequest () {
		}
		public abstract void Abort();
		public abstract IAsyncResult BeginGetRequestStream (AsyncCallback callback, object state);
		public abstract IAsyncResult BeginGetResponse (AsyncCallback callback, object state);
		public abstract Stream EndGetRequestStream (IAsyncResult asyncResult);
		public abstract WebResponse EndGetResponse (IAsyncResult asyncResult);
		internal virtual IAsyncResult BeginGetResponse (AsyncCallback callback, object state, bool policy) {
			return BeginGetResponse (callback, state);
		}
		public static WebRequest Create (string requestUriString) {
			return Create (new Uri (requestUriString));
		}
		public static WebRequest Create (Uri uri) {
			if (uri == null)
				throw new ArgumentNullException ("uri");
			if (!uri.IsAbsoluteUri)
				throw new InvalidOperationException ("This operation is not supported for a relative URI.");
			IWebRequestCreate creator = null;
			int n = -1;
			foreach (KeyValuePair<string, IWebRequestCreate> kvp in registred_prefixes) {
				string key = kvp.Key;
				if ((key.Length > n) && uri.AbsoluteUri.StartsWith (key)) {
					creator = kvp.Value;
					n = key.Length;
				}
			}
			string scheme = uri.Scheme;
			if ((scheme == "http" && n <= 5) || (scheme == "https" && n <= 6))
				creator = default_creator;
			if (creator == null)
				throw new NotSupportedException (string.Format ("Scheme {0} not supported", scheme));
			return creator.Create (uri);
		}
		public static HttpWebRequest CreateHttp (string requestUriString) {
			return CreateHttp (new Uri (requestUriString));
		}
		public static HttpWebRequest CreateHttp (Uri uri) {
			if (uri == null)
				throw new ArgumentNullException ("uri");
			if (!uri.IsAbsoluteUri)
				throw new InvalidOperationException ("Uri is not absolute.");
			switch (uri.Scheme) {
			case "http":
			case "https":
				return (HttpWebRequest) client_creator.Create (uri);
			default:
				throw new NotSupportedException (string.Format ("Scheme {0} not supported", uri.Scheme));
			}
		}
		public static bool RegisterPrefix (string prefix, IWebRequestCreate creator) {
			if (prefix == null)
				throw new ArgumentNullException ("prefix");
			if (creator == null)
				throw new ArgumentNullException ("creator");
			Uri uri;
			if (Uri.TryCreate (prefix, UriKind.Absolute, out uri)) {
				prefix = uri.Scheme + Uri.SchemeDelimiter + uri.DnsSafeHost;
			}
			if ((String.Compare (prefix, "http:", StringComparison.OrdinalIgnoreCase) == 0) ||
			    (String.Compare (prefix, "https:", StringComparison.OrdinalIgnoreCase) == 0))
				return false;
			if (registred_prefixes.ContainsKey (prefix))
				return false;
			registred_prefixes.Add (prefix, creator);
			return true;
		}
		static Exception NotImplemented () {
			return new NotImplementedException ();
		}
	}
}