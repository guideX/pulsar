// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
//using System.Collections.Generic;
using Pulsar.IO;
//using System.Linq;
//using System.Xml;
namespace Pulsar.Communications.Policy {
	partial class FlashCrossDomainPolicy {
		static bool ReadBooleanAttribute (string attribute)
		{
			switch (attribute) {
			case null:
			case "true":
				return true;
			case "false":
				return false;
			default:
				throw new XmlException ();
			}
		}
		static AllowAccessFrom CreateAllowAccessFrom (XmlReader reader) {
			int n = reader.AttributeCount;
			string domain = reader.GetAttribute ("domain");
			if (domain != null)
				n--;
			string secure = reader.GetAttribute ("secure");
			if (secure != null)
				n--;
			if (n != 0)
				throw new XmlException ("unknown/unsupported attributes");
			return new AllowAccessFrom () { Domain = domain, Secure = ReadBooleanAttribute (secure) };
		}
		static AllowHttpRequestHeadersFrom CreateAllowHttpRequestHeadersFrom (XmlReader reader)
		{
			int n = reader.AttributeCount;
			string domain = reader.GetAttribute ("domain");
			if (domain != null)
				n--;
			string secure = reader.GetAttribute ("secure");
			if (secure != null)
				n--;
			string headers = reader.GetAttribute ("headers");
			if (headers != null)
				n--;
			if (n != 0)
				throw new XmlException ("unknown/unsupported attributes");

			var h = new AllowHttpRequestHeadersFrom () { Domain = domain, Secure = ReadBooleanAttribute (secure) };
			h.Headers.SetHeaders (headers);
			return h;
		}

		// only "permitted-cross-domain-policies" attribute is allowed - anything else is considered invalid
		static string GetSiteControl (XmlReader reader)
		{
			int n = reader.AttributeCount;
			string site = reader.GetAttribute ("permitted-cross-domain-policies");
			if (site != null)
				n--;
			if (n != 0)
				throw new XmlException ("unknown/unsupported attributes");
			return site;
		}

		static public ICrossDomainPolicy FromStream (Stream stream)
		{
			FlashCrossDomainPolicy cdp = new FlashCrossDomainPolicy ();

			// Silverlight accepts whitespaces before the XML - which is invalid XML
			StreamReader sr = new StreamReader (stream);
			while (Char.IsWhiteSpace ((char) sr.Peek ()))
				sr.Read ();

			XmlReaderSettings policy_settings = new XmlReaderSettings ();
			policy_settings.DtdProcessing = DtdProcessing.Ignore;
			using (XmlReader reader = XmlReader.Create (sr, policy_settings)) {

				reader.MoveToContent ();
				if (reader.HasAttributes || reader.IsEmptyElement) {
					reader.Skip ();
					return null;
				}

				while (!reader.EOF) {
					reader.ReadStartElement ("cross-domain-policy", String.Empty);
					for (reader.MoveToContent (); reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent ()) {
						if (reader.NodeType != XmlNodeType.Element)
							throw new XmlException (String.Format ("Unexpected cross-domain-policy content: {0}", reader.NodeType));
						switch (reader.LocalName) {
						case "site-control":
							cdp.SiteControl = GetSiteControl (reader);
							reader.Skip ();
							break;
						case "allow-access-from":
							var a = CreateAllowAccessFrom (reader);
							cdp.AllowedAccesses.Add (a);
							reader.Skip ();
							break;
						case "allow-http-request-headers-from":
							var h = CreateAllowHttpRequestHeadersFrom (reader);
							cdp.AllowedHttpRequestHeaders.Add (h);
							reader.Skip ();
							break;
						default:
							reader.Skip ();
							return null;
						}
					}
					reader.ReadEndElement ();
					reader.MoveToContent ();
				}
			}

			// if none supplied set a default for headers
			if (cdp.AllowedHttpRequestHeaders.Count == 0) {
				var h = new AllowHttpRequestHeadersFrom () { Domain = "*", Secure = true };
				h.Headers.SetHeaders (null); // defaults
				cdp.AllowedHttpRequestHeaders.Add (h);
			}
			return cdp;
		}
	}
}