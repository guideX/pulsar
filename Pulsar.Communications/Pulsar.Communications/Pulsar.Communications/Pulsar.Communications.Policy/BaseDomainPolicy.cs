// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
//using System.Collections.Generic;
//using Pulsar.IO;
//using System.Linq;
namespace Pulsar.Communications.Policy {
	abstract class BaseDomainPolicy : ICrossDomainPolicy {
		static string root;
		static Uri app;
#if !TEST
		static BaseDomainPolicy ()
		{
			ApplicationUri = new Uri (AppDomain.CurrentDomain.GetData ("xap_uri") as string);
		}
#endif
		static public Uri ApplicationUri { 
			get { return app; }
			set {
				app = value;
				root = null;
			}
		}

		static public string ApplicationRoot {
			get {
				if (root == null)
					root = CrossDomainPolicyManager.GetRoot (ApplicationUri);
				return root;
			}
		}

		public class Headers {

			sealed class PrefixComparer : IEqualityComparer<string> {

				public bool Equals (string x, string y)
				{
					int check_length = x.Length - 1;
					if ((x.Length > 0) && (x [check_length] == '*'))
						return (String.Compare (x, 0, y, 0, check_length, StringComparison.OrdinalIgnoreCase) == 0);

					return (String.Compare (x, y, StringComparison.OrdinalIgnoreCase) == 0);
				}

				public int GetHashCode (string obj)
				{
					return (obj == null) ? 0 : obj.GetHashCode ();
				}
			}

			static PrefixComparer pc = new PrefixComparer ();

			private List<string> list;

			public Headers ()
			{
			}

			public bool AllowAllHeaders { get; private set; }

			public bool IsAllowed (string[] headers)
			{
				if (AllowAllHeaders)
					return true;

				if (headers == null || headers.Length == 0)
					return true;

				return headers.All (s => list.Contains (s, pc));
			}

			public void SetHeaders (string raw)
			{
				if (raw == "*") {
					AllowAllHeaders = true;
					list = null;
				} else if (raw != null) {
					string [] headers = raw.Split (',');
					list = new List<string> (headers.Length + 1);
					list.Add ("Content-Type");
					for (int i = 0; i < headers.Length; i++) {
						string s = headers [i].Trim ();
						if (!String.IsNullOrEmpty (s))
							list.Add (s);
					}
				} else {
					AllowAllHeaders = false;
					list = new List<string> (1);
					list.Add ("Content-Type");
				}
			}
		}

		abstract public bool IsAllowed (WebRequest request);

		public Exception Exception {
			get; internal set;
		}
	}
}
