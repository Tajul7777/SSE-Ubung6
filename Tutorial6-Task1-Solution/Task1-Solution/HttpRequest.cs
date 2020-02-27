using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SSE
{
    public class HttpRequest
    {
        private HttpRequest() {}

        public static async Task<string> Get(Url url)
        {
            // resolve DNS
            var addrs = await Dns.GetHostAddressesAsync(url.Host);

            // extract a IPv4 from the list of available addresses (since tu-chemnitz.de
            // is not available through IPv6 yet)
            // a more general approach would be to try a request to each address
            // in the list in a return the result from the first successfull request
            IPAddress ipv4 = null;
            foreach (var addr in addrs)
            {  
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipv4 = addr;
                    break;
                }
            }

            // if there is no IPv4 in the list of addresses for the given host
            if (ipv4 == null)
            {
                throw new ArgumentException("Cannot resolve IPv4 for host: " + url.Host);
            }

            // construct HTTP request
            var request = "GET " + url.Path + "?" + url.Query + " HTTP/1.1\n" +
                          "Host: " + url.Host + "\n" +
                          "\n";

            // send TCP request
            // Note: updated TcpRequest implementation to also support IPv6 requests
            return await TcpRequest.Do(ipv4, url.Port, request);
        }
    }
}