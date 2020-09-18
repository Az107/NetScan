using System;
using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Sockets;

public class Scanner
{
	private IPAddress IP { get; set; }
	private String Mask { get; set; }
	private List<String> AllIps = new List<string>();
	private List<bool> AllIpsD = new List<bool>();
	private int threads = 0;
	private int ActiveThreads = 0;
	private IArp arp;

	//private List<Thread> ActiveThreads = new List<Thread>();
	public List<String> result = new List<string>();
	public int MaxThreads = 100;

	public delegate void FoundIp(String ip,String Mac);
	public event FoundIp foudIp;

	int last = 0;

	private void Loop()
	{

		Ping ping = new Ping();
		string ip;
		for (int i = last;i < AllIps.Count;i++)
		{
			ip = AllIps[i];
			if (AllIpsD[i]) continue;
			else if (ip == IP.ToString())
            {
				AllIpsD[i] = true;
				foudIp.Invoke(ip, "THIS DEVICE");
				continue;
			}
            else
            {
				AllIpsD[i] = true;
				if (TestIp(ping,ip))
				{
					result.Add(ip);
					Thread.Sleep(1000);
				}

            }
		}
		ActiveThreads --;
	}


	private bool TestIp(Ping ping,string ip)
    {
		bool result = false;
		PingReply reply = ping.Send(IPAddress.Parse(ip),100);
		if (reply.Status == IPStatus.Success)
		{
			string mac = arp.getMac(ip);
			result = true;
			foudIp.Invoke(ip,mac);
		}
		return result;
    }

	private void LoadIps()
    {
		
		String[] SiP = IP.ToString().Split('.');
		String prefix = $"{SiP[0]}.{SiP[1]}.{SiP[2]}";
		for (int i = 1;i < 255; i++)
        {
			AllIpsD.Add(false);
			AllIps.Add($"{prefix}.{i.ToString()}");
        }
    }

	private void ThreadMaker()
    {

		while (threads < MaxThreads)
		{
			threads++;
			ActiveThreads++;
			new Thread(new ThreadStart(Loop)).Start();
			Thread.Sleep(50);
		}
		ActiveThreads--;
	}

	private bool testCompleted()
    {
		bool result = false;
		if (AllIpsD[AllIpsD.Count - 1])
        {
			foreach(bool ipd in AllIpsD)
            {
				if (!ipd)
                {
					result = false;
					break;
                }
                else
                {
					result = true;
                }
            }
        }
		return result;
    }



	public List<String> Start()
    {
		ActiveThreads++;
		new Thread(new ThreadStart(ThreadMaker)).Start();

		int count = 0;
		while (ActiveThreads > 0 )
		{
			Thread.Sleep(1000);
			if (ActiveThreads <= 2) count++;
			else count = 0;

			if (count == 50) {
				break; 
			}
			else if (count == 5) {
				if (testCompleted()) break;
            }
		}

		return result;
	}


	void GetIps()
	{
		IPAddress Mask = null;
		Dictionary<IPAddress, IPAddress> Addrs = new Dictionary<IPAddress, IPAddress>();
		NetworkInterface[] ifaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface ifaz in ifaces)
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
				List<IPAddress> candidates = Addrs.Keys.Where(x => Regex.IsMatch(x.ToString(), @"^192\.168\.[0-3]\.")).ToList();
				if (candidates.Count == 0)
				{
					candidates = Addrs.Keys.Where(x => Regex.IsMatch(x.ToString(), @"^192\.168\.")).ToList();
				}
				if (candidates.Count > 0) IP = candidates[0];
				else IP = Addrs.Keys.ToList()[0];
			}
			Mask = Addrs[IP];

	}

	public Scanner(String ip)
	{
		IP = IPAddress.Parse(ip);
		arp = new Arp(ip);
		LoadIps();

	}
	public Scanner()
    {
		GetIps();
		arp = new Arp(IP.ToString());
		LoadIps();
	}
}
