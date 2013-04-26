// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System.Security;
namespace Pulsar.Communications.Policy {
	sealed class NoAccessPolicy : ICrossDomainPolicy {
		static SecurityException security_exception = new SecurityException ();
		static NotSupportedException not_supported_exception = new NotSupportedException ();
		Exception ex;
		public bool IsAllowed (WebRequest request) {
			ex = security_exception;
			foreach (string header in request.Headers) {
				if (String.Compare ("Content-Type", header, StringComparison.OrdinalIgnoreCase) != 0)
					ex = not_supported_exception;
			}
			return false;
		}
		public Exception Exception {
			get { return ex; }
		}
	}
}