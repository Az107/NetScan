using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
public class ArpLin : IArp
{
    public Dictionary<String, String> MacList {get; set;}
	
	String IP { get; set; }

	public string getMac(String ip){
		string mac = String.Empty;
		if (MacList.ContainsKey(ip)) mac = MacList[ip];
		return mac;
	}

	public void Init(String ip){
		MacList = new Dictionary<string, string>();
		IP = ip;
		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = "ARP",
			Arguments = "-a",
			WindowStyle = ProcessWindowStyle.Hidden,
			RedirectStandardOutput = true
		};
		process.StartInfo = startInfo;
		process.Start();
		while (!process.StandardOutput.EndOfStream)
		{
			string line = process.StandardOutput.ReadLine();
			if (line == string.Empty) continue;
			line = line.Trim().TrimStart().TrimEnd();
			if (Regex.IsMatch(line,@"^[a-zA-Z].*$")) continue;
			List<String> addr = line.Split().ToList();
			addr.RemoveAll(str => String.IsNullOrEmpty(str));
			if (!MacList.Keys.Contains(addr[0]))
			{

				MacList.Add(addr[0], addr[1]);
			}
			

		}
	}
    
}