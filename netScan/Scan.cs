using System;
using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Text;

public class Scanner
{
	private String IP { get; set; }
	private String Mask { get; set; }
	private List<String> AllIps = new List<string>();
	private List<bool> AllIpsD = new List<bool>();
	private int threads = 0;
	private int ActiveThreads = 0;

	//private List<Thread> ActiveThreads = new List<Thread>();
	public List<String> result = new List<string>();
	public int MaxThreads = 100;

	public delegate void FoundIp(String ip);
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
		PingReply reply = ping.Send(IPAddress.Parse(ip),1000);
		if (reply.Status == IPStatus.Success)
		{
			result = true;
			foudIp.Invoke(ip);
		}
		return result;
    }

	private void LoadIps()
    {
		
		String[] SiP = IP.Split('.');
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
		return false;
    }

	public async void StartAsync()
    {

    }

	public List<String> Start()
    {
		ActiveThreads++;
		new Thread(new ThreadStart(ThreadMaker)).Start();


		while (ActiveThreads > 0 )
		{
			Thread.Sleep(1000);
		}
		return result;
	}

	public Scanner(String ip)
	{
		IP = ip;
		LoadIps();

	}
}
