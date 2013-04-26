// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
using System;
//using System.Collections.Generic;
using Pulsar.IO;
//using System.Linq;
//using System.Xml;
namespace Pulsar.Communications.Policy {
	partial class ClientAccessPolicy {
		static public ICrossDomainPolicy FromStream (Stream stream) {
			ClientAccessPolicy cap = new ClientAccessPolicy ();
			StreamReader sr = new StreamReader (stream);
			while (Char.IsWhiteSpace ((char) sr.Peek ()))
				sr.Read ();
			XmlReaderSettings policy_settings = new XmlReaderSettings ();
			policy_settings.DtdProcessing = DtdProcessing.Ignore;
			using (XmlReader reader = XmlReader.Create (sr, policy_settings)) {
				reader.MoveToContent ();
				if (reader.IsEmptyElement) {
					reader.Skip ();
					return null;
				}
				reader.ReadStartElement ("access-policy", String.Empty);
				for (reader.MoveToContent (); reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent ()) {
					if (reader.NodeType != XmlNodeType.Element)
						throw new XmlException (String.Format ("Unexpected access-policy content: {0}", reader.NodeType));

					if ((reader.LocalName != "cross-domain-access") || reader.IsEmptyElement) {
						reader.Skip ();
						continue;
					}

					reader.ReadStartElement ("cross-domain-access", String.Empty);
					for (reader.MoveToContent (); reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent ()) {
						if (reader.NodeType != XmlNodeType.Element)
							throw new XmlException (String.Format ("Unexpected access-policy content: {0}", reader.NodeType));

						if ((reader.Name != "policy") || reader.IsEmptyElement) {
							reader.Skip ();
							continue;
						}

						ReadPolicyElement (reader, cap);
					}
					reader.ReadEndElement ();
				}
				reader.ReadEndElement ();
			}
			return cap;
		}

		static void ReadPolicyElement (XmlReader reader, ClientAccessPolicy cap)
		{
			if (reader.HasAttributes || reader.IsEmptyElement) {
				reader.Skip ();
				return;
			}

			var policy = new AccessPolicy ();
			bool valid = true;

			reader.ReadStartElement ("policy", String.Empty);
			for (reader.MoveToContent (); reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent ()) {
				if (reader.NodeType != XmlNodeType.Element)
					throw new XmlException (String.Format ("Unexpected policy content: {0}", reader.NodeType));

				switch (reader.LocalName) {
				case "allow-from":
					ReadAllowFromElement (reader, policy);
					break;
				case "grant-to":
					ReadGrantToElement (reader, policy);
					break;
				default:
					valid = false;
					reader.Skip ();
					break;
				}
			}

			if (valid)
				cap.AccessPolicyList.Add (policy);
			reader.ReadEndElement ();
		}

		static void ReadAllowFromElement (XmlReader reader, AccessPolicy policy)
		{
			if (reader.IsEmptyElement) {
				reader.Skip ();
				return;
			}

			bool valid = true;
			string headers = null;
			string methods = null;		// new in SL3
			if (reader.HasAttributes) {
				int n = reader.AttributeCount;
				headers = reader.GetAttribute ("http-request-headers");
				if (headers != null)
					n--;
				methods = reader.GetAttribute ("http-methods");
				if (methods != null)
					n--;
				valid = (n == 0);
			}

			var v = new AllowFrom ();
			v.HttpRequestHeaders.SetHeaders (headers);
			v.AllowAnyMethod = (methods == "*"); // only legal value defined, otherwise restricted to GET and POST
			reader.ReadStartElement ("allow-from", String.Empty);
			for (reader.MoveToContent (); reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent ()) {
				if (reader.NodeType != XmlNodeType.Element)
					throw new XmlException (String.Format ("Unexpected allow-from content: {0}", reader.NodeType));
				if (!String.IsNullOrEmpty (reader.NamespaceURI)) {
					reader.Skip ();
					continue;
				}
				switch (reader.LocalName) {
				case "domain":
					var d = reader.GetAttribute ("uri");
					if (d == "*")
						v.AllowAnyDomain = true;
					else
						v.Domains.Add (d);
					reader.Skip ();
					break;
				default:
					valid = false;
					reader.Skip ();
					continue;
				}
			}
			if (valid)
				policy.AllowedServices.Add (v);
			reader.ReadEndElement ();
		}

		// only "path" and "include-subpaths" attributes are allowed - anything else is not considered
		static Resource CreateResource (XmlReader reader)
		{
			int n = reader.AttributeCount;
			string path = reader.GetAttribute ("path");
			if (path != null)
				n--;
			string subpaths = reader.GetAttribute ("include-subpaths");
			if (subpaths != null)
				n--;
			if ((n != 0) || !reader.IsEmptyElement)
				return null;

			return new Resource () { 
				Path = path,
				IncludeSubpaths = subpaths == null ? false : XmlConvert.ToBoolean (subpaths)
			};
		}

		static void ReadGrantToElement (XmlReader reader, AccessPolicy policy)
		{
			var v = new GrantTo ();
			bool valid = true;

			if (reader.HasAttributes || reader.IsEmptyElement) {
				reader.Skip ();
				return;
			}

			reader.ReadStartElement ("grant-to", String.Empty);
			for (reader.MoveToContent (); reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent ()) {
				if (reader.NodeType != XmlNodeType.Element)
					throw new XmlException (String.Format ("Unexpected grant-to content: {0}", reader.NodeType));
				if (!String.IsNullOrEmpty (reader.NamespaceURI)) {
					reader.Skip ();
					continue;
				}

				switch (reader.LocalName) {
				case "resource":
					var r = CreateResource (reader);
					if (r == null)
						valid = false;
					else
						v.Resources.Add (r);
					break;
				case "socket-resource":
					// ignore everything that is not TCP
					if (reader.GetAttribute ("protocol") != "tcp")
						break;
					// we can merge them all together inside a policy
					policy.PortMask |= ParsePorts (reader.GetAttribute ("port"));
					break;
				default:
					valid = false;
					break;
				}
				reader.Skip ();
			}
			if (valid)
				policy.GrantedResources.Add (v);
			reader.ReadEndElement ();
		}

		// e.g. reserved ? 4534-4502
		static long ParsePorts (string ports)
		{
			long mask = 0;
			int sep = ports.IndexOf ('-');
			if (sep >= 0) {
				// range
				ushort from = ParsePort (ports.Substring (0, sep));
				ushort to = ParsePort (ports.Substring (sep + 1));
				for (int port = from; port <= to; port++)
					mask |= (long) (1ul << (port - AccessPolicy.MinPort));
			} else {
				// single
				ushort port = ParsePort (ports);
				mask |= (long) (1ul << (port - AccessPolicy.MinPort));
			}
			return mask;
		}

		static ushort ParsePort (string s)
		{
			ushort port;
			if (!UInt16.TryParse (s, out port) || (port < AccessPolicy.MinPort) || (port > AccessPolicy.MaxPort))
				throw new XmlException ("Invalid port");
			return port;
		}
	}
}