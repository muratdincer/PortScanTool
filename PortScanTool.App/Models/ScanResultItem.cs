using PortScanTool.Core.Models;

namespace PortScanTool.App.Models
{
    public class ScanResultItem
    {
        public string IP_Address { get; set; }
        public int Port { get; set; }
        public PortStatus Status { get; set; }
    }
}
