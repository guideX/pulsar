// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
namespace Pulsar.Communications.NetworkInformation {
	public abstract class NetworkInterface {
		protected NetworkInterface () {
		}
		public static bool GetIsNetworkAvailable () {
			return NetworkChange.moon_network_service_get_is_network_available (NetworkChange.runtime_get_network_service ());
		}
	}
}