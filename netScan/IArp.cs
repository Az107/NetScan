using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

public interface IArp
{

    public Dictionary<String, String> MacList {get; set;}
    void Init(String IP);
    String getMac(String ip);

	
}
