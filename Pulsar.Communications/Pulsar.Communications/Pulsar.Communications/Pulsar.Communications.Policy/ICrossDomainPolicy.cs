// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications.Policy {
	interface ICrossDomainPolicy {
		bool IsAllowed (WebRequest request);
		Exception Exception { get; }
	}
}