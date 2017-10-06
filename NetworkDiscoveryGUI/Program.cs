using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Web;
using System.Management;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace NetworkDiscoveryGUI
{
    class Program
    {
        static CountdownEvent countdown;
        static int upCount = 0;
        static object lockObj = new object();
        const bool resolveNames = true;
        static public List<InfoToSend> AllInfo = new List<InfoToSend>();
        static int khaaap = 0;

        public static List<InfoToSend> Discovery()
        {
            while(AllInfo.Count !=  0)
            {
                AllInfo.RemoveAt(0);
            }
            GetAllMacAddressesAndIppairs();
            foreach (var item in AllInfo)
            {
                startScan(item.ipAddress);
            }
            getMyPC();
            string[] arr = { "local", "two", "three" };
            checkArgs(arr);
            checkArgs(arr);
            GetManufacturers();
            return AllInfo;
        }
        

        static void startScan(string ip)
        {
            countdown = new CountdownEvent(1);
            new Thread(delegate ()
            {
                try
                {
                    Ping p = new Ping();
                    p.PingCompleted += new PingCompletedEventHandler(pingDone);
                    p.SendAsync(ip, 100, ip);
                }
                catch (SocketException ex)
                {
                    InfoToSend s = AllInfo.Find(x => x.ipAddress == ip);
                    s.displayName = ip;
                }
            }).Start();
            countdown.Wait();
        }

        static void pingDone(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                if (resolveNames)
                {
                    string name = null;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;

                    }
                    catch (SocketException ex)
                    {
                        name = ip;
                    }
                    string opers = "";
                    if (e.Reply.Options.Ttl == 64)
                    {
                        opers = "Linux/Unix/Android";
                    }
                    else if (e.Reply.Options.Ttl == 128)
                    {
                        opers = "Windows";
                    }
                    else
                    {
                        opers = "Solaris/AIX";
                    }
                    InfoToSend s = AllInfo.Find(x => x.ipAddress == ip);
                    s.displayName = name;
                    s.os = opers;
                }
                lock(lockObj)
                {
                    upCount++;
                }
            }
            else
            {
                InfoToSend s = AllInfo.Find(x => x.ipAddress == ip);
                s.displayName = ip;
                s.os = "(No Reply)";
            }
            countdown.Signal();
        }

        static void GetManufacturers()
        {

            string manufacturer = "";
            foreach (var item in AllInfo)
            {
                if (item.macAddress != null)
                {
                    manufacturer = (MacAPI.GetMAC(item.macAddress)).result.company;
                }
                InfoToSend s = AllInfo.Find(x => x.ipAddress == item.ipAddress);
                s.manf = manufacturer;
            }
        }

        public static void GetAllMacAddressesAndIppairs()
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})\s*(?<type>([a-z]){1})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                string type = m.Groups["type"].Value;
                if (type == "d")
                {
                    AllInfo.Add(new InfoToSend()
                    {
                        macAddress = m.Groups["mac"].Value,
                        ipAddress = m.Groups["ip"].Value,
                    });
                }
            }
        }

        static void getMyPC()
        {
            var wmi = getOsInfo();
            string OsName = ((string)wmi["Caption"]).Trim();
            string hostName = Dns.GetHostName();
            string DisplayName = Environment.MachineName;
            string MacAddress = getMacAddress();
            string IpAddress = Dns.GetHostByName(hostName).AddressList[0].ToString();
            AllInfo.Add(new InfoToSend()
            {
                displayName = DisplayName,
                macAddress = MacAddress,
                ipAddress = IpAddress,
                os = OsName
            });
        }
        static ManagementObject getOsInfo()
        {
            return new ManagementObjectSearcher("select * from Win32_OperatingSystem")
               .Get()
               .Cast<ManagementObject>()
               .First();
        }
        static string getMacAddress()
        {
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMOS.Get();
            string macAddress = String.Empty;
            foreach (ManagementObject objMO in objMOC)
            {
                object tempMacAddrObj = objMO["MacAddress"];

                if (tempMacAddrObj == null) //Skip objects without a MACAddress
                {
                    continue;
                }
                if (macAddress == String.Empty) // Only return MAC Address from first card that has a MAC Address
                {
                    macAddress = tempMacAddrObj.ToString();
                }
                objMO.Dispose();
            }
            return macAddress;
        }

        static void checkArgs(string[] args)
        {
            if (args[0] == "local")
            {
                string base_ip = defGateway();
                if (base_ip != null)
                {
                    // Functions.log(string.Format("Network Gateway: {0}", base_ip), 5);
                    startScan2(Regex.Match(base_ip, "(\\d+.\\d+.\\d+.)").Groups[0].Value);
                }
                else
                {
                    //  Functions.log(string.Format("Unable to get ip address."), 2);
                }
            }
            else
            {
                startScan2(Regex.Match(args[0], "(\\d+.\\d+.\\d+.)").Groups[0].Value);
            }
        }

        static void startScan2(string ipBase)
        {
            countdown = new CountdownEvent(1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 1; i < 255; i++)
            {
                string ip = ipBase + i.ToString();
                new Thread(delegate()
                {
                    try
                    {
                        Ping p = new Ping();
                        p.PingCompleted += new PingCompletedEventHandler(pingDone2);
                        countdown.AddCount();
                        p.SendAsync(ip, 100, ip);
                        //PingReply o = p.Send(ip,100);
                    }
                    catch (SocketException ex)
                    {
                        //  Functions.log (string.Format("Could not contact {0}", ip), 3);
                    }
                }).Start();
            }
            countdown.Signal();
            countdown.Wait();
            sw.Stop();
            //TimeSpan span = new TimeSpan(sw.ElapsedTicks);
            //   Functions.log(string.Format("Took {0} milliseconds. {1} hosts active.", sw.ElapsedMilliseconds, upCount), 1);
            //DisplayAll(AllInfo);
        }

        static void pingDone2(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            //Functions.log(string.Format("{0}", e.Reply.Options.Ttl.ToString()), 2);
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                if (resolveNames)
                {
                    string name = null;
                    string ips = null;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                        ips = getMacByIp(ip);

                    }
                    catch (SocketException ex)
                    {
                        name = ip;
                    }

                    //  Functions.log(string.Format("{0} ({1}) is up: ({2} ms) MacAddress is: {3}, TTL is: {4}", ip, name, e.Reply.RoundtripTime, ips, e.Reply.Options.Ttl), 2);
                    string opers = "";
                    if (e.Reply.Options.Ttl == 64)
                    {
                        opers = "Linux/Unix/Android";
                    }
                    else if (e.Reply.Options.Ttl == 128)
                    {
                        opers = "Windows";
                    }
                    else
                    {
                        opers = "Solaris/AIX";
                    }
                    string manufacturer = "";
                    //if(ips != null )
                    //{
                    //    manufacturer = (MacAPI.GetMAC(ips)).result.company;
                    //}
                    InfoToSend s = AllInfo.Find(x => x.ipAddress == ip);
                    if (s == null)
                    {
                        AllInfo.Add(new InfoToSend()
                        {
                            displayName = name,
                            macAddress = ips,
                            ipAddress = ip,
                            os = opers,
                            manf = manufacturer
                        });
                    }
                    else
                    {
                        s.displayName = name;
                        s.os = opers;
                    }
                }
                else
                { //but it's reachable doe.
                    // Functions.log(string.Format("{0} is up: ({1} ms)", ip, e.Reply.RoundtripTime), 2);
                }
                lock (lockObj)
                {
                    upCount++;
                }
            }
            else if (e.Reply == null)
            {
                //Functions.log(string.Format("Pinging {0} failed. (Null Reply object?)", ip), 3);
            }
            countdown.Signal();
        }

        static string defGateway()
        {
            string ip = null;
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface f in interfaces)
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {

                        if (d.Address.ToString() != "::")
                        {
                            ip = d.Address.ToString();
                        }
                    }
                }
            return ip;
        }

        private static string getMacByIp(string ip)
        {
            var macIpPairs = GetAllMacAddressesAndIppair();
            int index = macIpPairs.FindIndex(x => x.IpAddress == ip);
            if (index >= 0)
            {
                return macIpPairs[index].MacAddress.ToUpper();
            }
            else
            {
                return null;
            }
        }

        public static List<MacIpPair> GetAllMacAddressesAndIppair()
        {
            List<MacIpPair> mip = new List<MacIpPair>();
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                mip.Add(new MacIpPair()
                {
                    MacAddress = m.Groups["mac"].Value,
                    IpAddress = m.Groups["ip"].Value
                });
            }

            return mip;
        }
        public struct MacIpPair
        {
            public string MacAddress;
            public string IpAddress;
        }
    }
}
