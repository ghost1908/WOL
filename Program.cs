using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;

namespace WOL
{
    public class Program
    {
        private static IEnumerable<WakeUpItem> wakeUpItems;

        public static void Main(string[] args)
        {
            byte[] magicPacket = new byte[102];
            string mac;
            IPAddress wakeUpIP;

            ReadSettings();
            ShowWakeUp(wakeUpItems);

            if (args.Length == 0)
            {
                Console.Write("Enter MAC address: ");
                magicPacket = Str2Mac(mac = Console.ReadLine());
                wakeUpIP = GetIp();
            }
            else if (args.Length == 1)
            {
                magicPacket = Str2Mac(mac = args[0]);
                wakeUpIP = GetIp();
            }
            else if (args.Length == 2)
            {
                magicPacket = Str2Mac(mac = args[0]);
                IPAddress.TryParse(args[1], out wakeUpIP);
            }
            else
            {
                Console.WriteLine("Usage: WOL {mac address} {ipaddress}\n");
                return;
            }

            if (wakeUpIP == null) wakeUpIP = IPAddress.Broadcast;

            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Send(magicPacket, magicPacket.Length, new IPEndPoint(wakeUpIP, 9));
                Console.WriteLine("Magic packet [{0}] sended to {1}\n", mac, wakeUpIP.ToString());
            }
        }

        protected static IPAddress GetIp()
        {
            IPAddress result;
            Console.Write("Enter IP address (empty - uses Broadcast address): ");
            IPAddress.TryParse(Console.ReadLine(), out result);
            return result;
        }

        private static byte[] Str2Mac(string s)
        {
            List<byte> arr = new List<byte>(102);

            string[] macs = s.Split(' ', ':', '-');

            for (int i = 0; i < 6; i++)
                arr.Add(0xFF);

            for (int j = 0; j < 16; j++)
                for (int i = 0; i < 6; i++)
                    arr.Add(Convert.ToByte(macs[i], 16));

            return arr.ToArray();
        }

        private static void ReadSettings()
        {
            XDocument xDoc = XDocument.Load("wol.xml");
            List<WakeUpItem> result = new List<WakeUpItem>();

            foreach (XElement el in xDoc.Root.Elements())
            {
                WakeUpItem item = new WakeUpItem();
                item.ID = Convert.ToInt32(el.Attribute("id").Value);
                item.MAC = el.Attribute("mac").Value;

                IPAddress getIpAddress;
                IPAddress.TryParse(el.Attribute("ip").Value, out getIpAddress);
                if (getIpAddress == null) getIpAddress = IPAddress.Broadcast;
                item.WakeUpIP = getIpAddress;

                item.Description = el.Attribute("description").Value;

                result.Add(item);
            }

            wakeUpItems = result;
        }

        private static void ShowWakeUp(IEnumerable<WakeUpItem> items)
        {
            Console.WriteLine(new String('-', 10));
            Console.WriteLine("ID\t| MAC\t| IP\t| Description");
            Console.WriteLine(new String('-', 10));
            foreach (WakeUpItem item in items)
            {
                Console.WriteLine("{0}\t| {1}\t| {2}\t| {3}", item.ID, item.MAC, item.WakeUpIP, item.Description);
            }
            Console.WriteLine(new String('-', 10));
        }
    }
}
