using System.Threading;
using System.Threading.Tasks;

namespace PortScanTool.Core.Models
{
    public class Worker
    {
        public Task Task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public Worker(Task task, CancellationTokenSource cancellationTokenSource)
        {
            Task = task;
            CancellationTokenSource = cancellationTokenSource;
            CancellationToken = cancellationTokenSource.Token;
        }
    }
}
