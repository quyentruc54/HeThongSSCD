using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace NovaAlert.Communication.Tcp
{
    static class Utility
    {
        public static byte[] FromHex(string hex)
        {
            hex = RefineHexText(hex);
            hex = Regex.Replace(hex, @"\s+", "");

            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        public static string ToHexText(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", " ");
        }

        const string HexPattern = @"([0-9a-fA-F]{2}\s*)+";
        public static bool IsHexString(string s)
        {
            return Regex.IsMatch(s, HexPattern);
        }

        public static string RefineHexText(string s)
        {
            s = Regex.Replace(s, @"[^0-9a-fA-F\s]+", " ");
            s = Regex.Replace(s, @"[\r\n]", " ");
            s = Regex.Replace(s, @"\s{2,}", " ");
            return s;
        }

        public static string GetAsciiString(byte[] data)
        {
            var ascii = System.Text.Encoding.ASCII.GetString(data);
            ascii = Regex.Replace(ascii, "\n", ".");
            return ascii;
        }

        public static IPAddress GetMyIP()
        {
            IPHostEntry host;
            IPAddress localIP = IPAddress.Loopback;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip;
                }
            }
            return localIP;
        }

        public static bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
