using System;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace NovaAlert.Common.Utils
{
    public static class PathHelper
    {
        public static string ConvertLocalFolderPathToIPBasedUNCPath(string localFolderName)
        {
            string ipBasedUNCPath = string.Empty;

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT Name FROM Win32_share WHERE path ='" +
            localFolderName.Replace("\\", "\\\\") + "'");
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            if (managementObjectCollection.Count != 0)
            {
                foreach (ManagementObject item in managementObjectCollection)
                {
                    String ComputerName = ReturnMachineIP().ToString();// use  Dns.GetHostName(); for computername instead of IP
                    ipBasedUNCPath = item["Name"] as String;
                    ipBasedUNCPath = "\\\\" + ComputerName + "\\" + ipBasedUNCPath;
                    return ipBasedUNCPath;
                }
            }
            return ipBasedUNCPath;
        }

        private static IPAddress ReturnMachineIP()
        {
            String hostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addr = ipEntry.AddressList;
            IPAddress ipV4 = null;
            foreach (IPAddress item in addr)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipV4 = item;
                    break;
                }

            }
            if (ipV4 == null)
            {
                throw new ApplicationException("You have no IP of Version 4.Server can not run witout it");
            }
            return ipV4;
        }

        public static string GetLocalPath(string uncPath)
        {
            try
            {
                // remove the "\\" from the UNC path and split the path
                uncPath = uncPath.Replace(@"\\", "");
                string[] uncParts = uncPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (uncParts.Length < 2)
                {
                    return "[UNRESOLVED UNC PATH: " + uncPath + "]";
                }

                // Get a connection to the server as found in the UNC path
                ManagementScope scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");
                // Query the server for the share name
                SelectQuery query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                // Get the path
                string path = string.Empty;
                foreach (ManagementObject obj in searcher.Get())
                {
                    path = obj["path"].ToString();
                }

                // Append any additional folders to the local path name
                if (uncParts.Length > 2)
                {
                    for (int i = 2; i < uncParts.Length; i++)
                    {
                        path = path.EndsWith(@"\") ? path + uncParts[i] : path + @"\" + uncParts[i];
                    }
                }

                return path;
            }
            catch (Exception ex)
            {
                return "[ERROR RESOLVING UNC PATH: " + uncPath + ": " + ex.Message + "]";
            }
        }
    }
}
