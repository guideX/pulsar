// Pulsar Operating System v0.2
// guideX - Team Nexgen - 2012
//using System;
//using System.Collections;
using Pulsar;
using Pulsar.Communications.Sockets;
//using System.Runtime.CompilerServices;
namespace Pulsar.Communications {
  internal static class Dns {
    [MethodImplAttribute(MethodImplOptions.InternalCall)]
    private extern static bool GetHostByName_internal(string host, out string h_name, out string[] h_aliases, out string[] h_addr_list);
    internal static IPAddress[] GetHostAddresses(string hostNameOrAddress) {
      if (hostNameOrAddress == null)
        throw new ArgumentNullException("hostNameOrAddress");
      if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
        throw new ArgumentException("Addresses 0.0.0.0 (IPv4) " +
          "and ::0 (IPv6) are unspecified addresses. You " +
          "cannot use them as target address.",
          "hostNameOrAddress");
      IPAddress addr;
      if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out addr))
        return new IPAddress[1] { addr };
      string h_name;
      string[] h_aliases, h_addrlist;
      bool ret = GetHostByName_internal(hostNameOrAddress, out h_name, out h_aliases, out h_addrlist);
      if (ret == false)
        throw new SocketException(11001);
      IPHostEntry entry = hostent_to_IPHostEntry(h_name, h_aliases, h_addrlist);
      return entry.AddressList;
    }

    private static IPHostEntry hostent_to_IPHostEntry(string h_name, string[] h_aliases, string[] h_addrlist) {
      IPHostEntry he = new IPHostEntry();
      ArrayList addrlist = new ArrayList();

      he.HostName = h_name;
      he.Aliases = h_aliases;
      for (int i = 0; i < h_addrlist.Length; i++) {
        try {
          IPAddress newAddress = IPAddress.Parse(h_addrlist[i]);

          if ((Socket.SupportsIPv6 && newAddress.AddressFamily == AddressFamily.InterNetworkV6) ||
              (Socket.SupportsIPv4 && newAddress.AddressFamily == AddressFamily.InterNetwork))
            addrlist.Add(newAddress);
        }
        catch (ArgumentNullException) {
        }
      }

      if (addrlist.Count == 0)
        throw new SocketException(11001);

      he.AddressList = addrlist.ToArray(typeof(IPAddress)) as IPAddress[];
      return he;
    }
  }
}