using PortScanTool.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanTool.Core
{
    public class Scanner
    {
        private const int minPortNumber = 1;
        private const int maxPortNumber = 65535;

        private int InitialWorkerCount { get; set; }
        private int CurrentWorkerCount { get; set; }

        private CancellationTokenSource ScanCancellationTokenSource { get; set; }
        private CancellationToken ScanCancellationToken { get; set; }

        public ConcurrentQueue<ScannerQueueObject> ScannerQueues { get; set; }

        private List<Worker> Workers;

        public event EventHandler<PortCheckCompletedEventArgs> OnPortCheckCompleted;
        public event EventHandler<EventArgs> OnStarted;
        public event EventHandler<EventArgs> OnCanceled;
        public event EventHandler<EventArgs> OnFinished;

        public Scanner(int initialWorkerCount)
        {
            CurrentWorkerCount = 0;
            InitialWorkerCount = initialWorkerCount;

            Initialize();
        }

        private void Initialize()
        {
            ScanCancellationTokenSource = new CancellationTokenSource();
            ScanCancellationToken = ScanCancellationTokenSource.Token;

            ScannerQueues = new ConcurrentQueue<ScannerQueueObject>();

            Workers = new List<Worker>();
        }

        public async Task AddQueueAsync(IEnumerable<IPAddress> ipAddresses)
        {
            await Task.Run(() =>
            {
                foreach (var ipAddress in ipAddresses)
                {
                    for (int port = minPortNumber; port < maxPortNumber; port++)
                    {
                        ScannerQueues.Enqueue(new ScannerQueueObject
                        {
                            IPAddress = ipAddress,
                            Port = port,
                            Status = PortStatus.Unknown
                        });
                    }
                }
            });
        }

        public async Task StartAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    while (CurrentWorkerCount != InitialWorkerCount)
                    {
                        IncreaseWorker();
                    }

                    OnStarted?.Invoke(this, new EventArgs { });

                }, ScanCancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task StopAsync()
        {
            await Task.Run(() =>
            {
                ScanCancellationTokenSource.Cancel();

                OnCanceled?.Invoke(this, new EventArgs { });
            });
        }

        public void IncreaseWorker()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var task = CreateWorker(cancellationTokenSource.Token);

            var worker = new Worker(task, cancellationTokenSource);

            Workers.Add(worker);

            CurrentWorkerCount++;
        }
        public void DecreaseWorker()
        {
            var lastworker = Workers.LastOrDefault();
            lastworker.CancellationTokenSource.Cancel();
            Workers.Remove(lastworker);

            CurrentWorkerCount--;
        }

        public async Task ChangeWorkerCountAsync(int workerCount)
        {
            try
            {
                await Task.Run(() =>
                {
                    while (CurrentWorkerCount != workerCount)
                    {
                        if (CurrentWorkerCount < workerCount) IncreaseWorker();
                        else DecreaseWorker();
                    }
                });
            }
            catch (AggregateException)
            {

            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Scan(ScannerQueueObject scannerQueueObject)
        {
            ScanCancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var tcpClient = new TcpClient())
                {
                    var connectResult = tcpClient.ConnectAsync(scannerQueueObject.IPAddress, scannerQueueObject.Port).Wait(TimeSpan.FromSeconds(2).Milliseconds, ScanCancellationToken);

                    scannerQueueObject.Status = connectResult ? PortStatus.Open : PortStatus.Closed;
                }
            }
            catch (Exception)
            {
                scannerQueueObject.Status = PortStatus.Closed;
            }

            OnPortCheckCompleted?.Invoke(this, new PortCheckCompletedEventArgs
            {
                IPAddress = scannerQueueObject.IPAddress,
                Port = scannerQueueObject.Port,
                Status = scannerQueueObject.Status
            });
        }

        private Task CreateWorker(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                while (!ScannerQueues.IsEmpty && ScannerQueues.TryDequeue(out ScannerQueueObject scannerQueueObject))
                {
                    ScanCancellationToken.ThrowIfCancellationRequested();
                    cancellationToken.ThrowIfCancellationRequested();

                    Scan(scannerQueueObject);
                }

                if (ScannerQueues.IsEmpty)
                {
                    OnFinished?.Invoke(this, new EventArgs { });
                }
            }, ScanCancellationToken);
        }
    }
}
