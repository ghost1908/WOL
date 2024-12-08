using System.Net;

namespace WOL
{
    public class WakeUpItem
    {
        public int ID { get; set; }
        public string MAC { get; set; }
        public IPAddress WakeUpIP { get; set; }
        public string Description { get; set; }
    }
}
