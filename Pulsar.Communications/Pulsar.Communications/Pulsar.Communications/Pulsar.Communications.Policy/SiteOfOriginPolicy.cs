// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications.Policy {
	sealed class SiteOfOriginPolicy : ICrossDomainPolicy {
		public bool IsAllowed (WebRequest request) {
			return true;
		}
		static public bool HasSameOrigin (Uri a, Uri b) {
			return ((a.Scheme == b.Scheme) && (a.DnsSafeHost == b.DnsSafeHost) &&
				((a.Port == -1) || (b.Port == -1) || (a.Port == b.Port)));
		}
		public Exception Exception {
			get { return null; }
		}
	}
}