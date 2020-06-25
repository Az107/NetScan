using System;
using System.Collections.Generic;
using System.Diagnostics;



public class Arp
{
	Dictionary<String, String> MacList = new Dictionary<string, string>();

	public Arp()
	{
		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo("arp", "-a");


	}
}
