using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Net.Sockets;
using LukeSkywalker.IPNetwork;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;

namespace NetCopy
{
    class ShareDiscoveryWorker
    {
        private List<string> _ipAddressesSearched = new List<string>();
        private List<string> _shareNamesToSearch = new List<string> { "a$", "b$", "c$", "d$", "e$", "f$", "g$", "h$", "i$", "j$", "k$", "l$", "m$", "n$", "o$", "p$", "q$", "r$", "s$", "t$", "u$", "v$", "w$", "x$", "y$", "z$"};
        private List<int> _filesharingPorts = new List<int> { 445, 135, 136, 137, 138, 139 };


        private void SearchActiveDirectory()
        {
            var domainName = Domain.GetCurrentDomain().Name;

            var ldapString = new StringBuilder("LDAP://");

            var domainNameParts = domainName.Split('.');

            for (int i = 0; i < domainNameParts.Length; i++ )
                ldapString.AppendFormat("DC={0}{1}", domainNameParts[i], i + 1 < domainNameParts.Length ? ",":"");
            
            var directoryEntry = new DirectoryEntry(ldapString.ToString());
            var directorySearcher = new DirectorySearcher(directoryEntry, "(objectclass=computer)");
            SearchResultCollection results = directorySearcher.FindAll();

            var machineNames = new List<string>();
            foreach (SearchResult result in results)
                machineNames.Add(result.GetDirectoryEntry().Name.Replace("CN=", ""));

            Parallel.ForEach(machineNames, machineName => 
            {
                FindShares(machineName, true);
            });
        }

        private void SearchLan()
        {
            var t = Dns.GetHostEntry(Dns.GetHostName());

            var currentIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            foreach(var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up && nic.Supports(NetworkInterfaceComponent.IPv4) && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    var nicProps = nic.GetIPProperties();
                    foreach (var address in nicProps.UnicastAddresses)
                    {
                        var ipNetwork = IPNetwork.Parse(address.Address, address.IPv4Mask);

                        var subNet = IPNetwork.Subnet(ipNetwork, 32);

                        foreach (var addr in subNet)
                        {
                            FindShares(addr.FirstUsable.ToString(), false);
                        }
                    }
                }
            }
        }

        private void SearchLocalNetworks()
        {
            
        }

        private void FindShares(string machineName, bool dnsLookup)
        {
            List<string> addresses = new List<string>();

            if (dnsLookup)
            {

                System.Net.IPHostEntry entry;
                try
                {
                    entry = System.Net.Dns.GetHostEntry(machineName);
                }
                catch (SocketException)
                {
                    //Stupid but GetHostEntry throws an exception if the host isn't found
                    return;
                }

                if (entry == null)
                    return;

                addresses.AddRange(entry.AddressList.Select(a => a.ToString()));
            }
            else
            {
                var addThisAddress = false;

                //Console.WriteLine(string.Format("Scanning {0}", machineName));

                Parallel.ForEach(_filesharingPorts, (port, state) => 
                {
                    var client = new TcpClient();
                    try
                    {
                        
                        var result = client.BeginConnect(machineName, port, null, null);

                        var successful = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));

                        if (successful && client.Connected)
                        {
                            //Console.WriteLine(string.Format("Adding the address {0}",machineName));
                            addThisAddress = true;
                            state.Break();
                        }
                    }
                    catch
                    {
                        
                    }
                    finally
                    {
                        client.Close();
                    }
                });
                //Console.WriteLine(string.Format("Done Scanning {0}", machineName));

                if (addThisAddress)
                    addresses.Add(machineName);
            }


            foreach (var address in addresses)
            {
                var addressString = address.ToString();
                //If we've already looked at this address move onto the next one
                if (_ipAddressesSearched.Contains(addressString))
                    continue;
                _ipAddressesSearched.Add(addressString);

                Parallel.ForEach(_shareNamesToSearch, shareName =>
                {
                    var fullPath = string.Format("\\\\{0}\\{1}", addressString, shareName);

                    //Console.WriteLine(string.Format("Trying path {0}", fullPath));

                    if (Directory.Exists(Path.Combine(addressString, fullPath)))
                        ShareQueue.Instance.AddShare(fullPath);
                });
            }
        }

        internal void Start()
        {
            if (SessionConfiguration.Instance.SearchAD)
                SearchActiveDirectory();
            if (SessionConfiguration.Instance.SearchLan)
                SearchLan();
            if (SessionConfiguration.Instance.SearchNearbyNetworks)
                SearchLocalNetworks();
        }
    }
}
