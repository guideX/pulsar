// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System.Runtime.InteropServices;
namespace Pulsar.Communications.NetworkInformation {
	public abstract class NetworkChange {
		internal delegate void NetworkStateChangedCallback (IntPtr sender, IntPtr data);
		static NetworkChange () {
			state_changed_callback = new NetworkStateChangedCallback (StateChangedCallback);
			moon_network_service_set_network_state_changed_callback (runtime_get_network_service(),
										 state_changed_callback,
										 IntPtr.Zero);
		}
		static NetworkStateChangedCallback state_changed_callback;
		static void StateChangedCallback (IntPtr sender, IntPtr data) {
			try {
				NetworkAddressChangedEventHandler h = NetworkAddressChanged;
				if (h != null)
					h (null, EventArgs.Empty);
			} catch (Exception ex) {
				try {
					Console.WriteLine ("Unhandled exception: {0}", ex);
				} catch {
					// Ignore
				}
			}
		}
		protected NetworkChange () {
		}
		public static event NetworkAddressChangedEventHandler NetworkAddressChanged;
		[DllImport ("moon")]
		internal extern static IntPtr runtime_get_network_service ();
		[DllImport ("moon")]
		internal extern static void moon_network_service_set_network_state_changed_callback (IntPtr service,
												     NetworkStateChangedCallback handler, IntPtr data);
		[DllImport ("moon")]
		internal extern static bool moon_network_service_get_is_network_available (IntPtr service);
	}
}