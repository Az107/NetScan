using System;
using System.Collections.Generic;
using System.Diagnostics;



public class Arp
{
	Dictionary<String, String> MacList = new Dictionary<string, string>();

	public Arp()
	{
		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = "ARP.exe",
			Arguments = "-a",
			WindowStyle = ProcessWindowStyle.Hidden,
			RedirectStandardOutput = true
		};
		process.StartInfo = startInfo;
		process.Start();




	}
}
