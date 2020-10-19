using System.Net;

namespace PortScanTool.Core.Models
{
    public class ScannerQueueObject
    {
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
        public PortStatus Status { get; set; }
    }
}
