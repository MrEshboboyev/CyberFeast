using System.Net;
using System.Net.Sockets;

namespace BuildingBlocks.Core.Utils;

/// <summary>
/// Provides utility methods for working with IP addresses.
/// </summary>
public static class IpUtilities
{
    /// <summary>
    /// Retrieves the IP address of the current machine.
    /// </summary>
    /// <returns>The IP address as a string, or an empty string if no IPv4 address is found.</returns>
    public static string GetIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        }

        return string.Empty;
    }
}