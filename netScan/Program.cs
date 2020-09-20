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

        private static Scanner scanner;
        private static IPAddress iP;
        static void printIp(string ip,string mac)
        {
            Console.WriteLine($"{ip}{new string(' ',15-ip.Length)}{(mac == string.Empty? "":"------   " + mac)}");
        }

        static void Main(string[] args)
        {
            bool arguments = false;
            
            if (arguments && args.Contains("-ip"))
            {
                int i = Array.IndexOf(args, "-ip");
                if (IPAddress.TryParse(args[i + 1],out iP))
                {
                    scanner = new Scanner(args[i+1]);

                }
            }
            else
            {
                scanner = new Scanner();
            }

            Console.WriteLine("Starting scan...");

            if (arguments && (args.Contains("-MaxThreads") || args.Contains("-mt"))) 
            {
                int i = Array.IndexOf(args, "-ip");
                scanner.MaxThreads = int.Parse(args[i + 1]);
            }
            scanner.foudIp += printIp;
            scanner.Start();
            Console.WriteLine($"Finnished {scanner.IpResult.Count} ips found");
            

        }
    }
}
