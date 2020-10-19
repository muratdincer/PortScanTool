using System;
using System.Net;

namespace PortScanTool.Core.Models
{
    public class PortCheckCompletedEventArgs :EventArgs
    {
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
        public PortStatus Status { get; set; }
    }
}
