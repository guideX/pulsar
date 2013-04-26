// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications.Policy {
	sealed class PolicyDownloadPolicy : ICrossDomainPolicy {
		public bool IsAllowed (WebRequest request) {
			return IsLocalPathPolicy (request.RequestUri);
		}
		static public bool IsLocalPathPolicy (Uri uri) {
			string local = uri.LocalPath;
			if (String.CompareOrdinal (local, CrossDomainPolicyManager.ClientAccessPolicyFile) == 0)
				return true;
			if (String.CompareOrdinal (local, CrossDomainPolicyManager.CrossDomainFile) == 0)
				return true;
			return false;
		}
		public Exception Exception {
			get { return null; }
		}
	}
}
