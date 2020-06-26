using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Sockets;

namespace netScan
{
    class Program
    {

        private static IPAddress IP = null;
        private static IPAddress Mask = null;
        private static Dictionary<IPAddress, IPAddress> Addrs = new Dictionary<IPAddress, IPAddress>();
        static void GetIps()
        {

            NetworkInterface[] ifaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach(NetworkInterface ifaz in ifaces)
            {
                IPAddress ip, mask;
                UnicastIPAddressInformation unicast = ifaz.GetIPProperties().UnicastAddresses.Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
                ip = unicast.Address;
                mask = unicast.IPv4Mask;
                Addrs.Add(ip, mask);


            }

            if (Addrs.Keys.Count == 1) IP = Addrs.Keys.ToList()[0];
            else
            {
                List<IPAddress> candidates = Addrs.Keys.Where(x => Regex.IsMatch(x.ToString(),@"^192\.168\.[0-3]\.")).ToList();
                if (candidates.Count > 0) IP = candidates[0];
                else IP = Addrs.Keys.ToList()[0];
            }
            Mask = Addrs[IP];


        }

        static void printIp(string ip)
        {
            Console.WriteLine(ip);
        }

        static void Main(string[] args)
        {

            if (args.Contains("-ip"))
            {
                int i = Array.IndexOf(args, "-ip");
                IP = IPAddress.Parse(args[i + 1]);
            }
            else
            {
                GetIps();
            }

            Console.WriteLine("Starting scan...");
            Scanner scanner = new Scanner(IP.ToString());

            if (args.Contains("-MaxThreads") || args.Contains("-mt")) { }
            {
                int i = Array.IndexOf(args, "-ip");
                scanner.MaxThreads = int.Parse(args[i + 1]);
            }



            scanner.foudIp += printIp;
            scanner.Start();
            Console.WriteLine("finnished");
            

        }
    }
}
