using PortScanTool.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanTool.Core
{
    /// <summary>
    /// Scanner object is a class to scan tcp ports 1 to 65535 of specified ip addresses
    /// </summary>
    public class Scanner : IDisposable
    {
        /// <summary>
        /// Lowest port number to start scanning
        /// </summary>
        private const int minPortNumber = 1;
        /// <summary>
        /// Highest port number to start scanning
        /// </summary>
        private const int maxPortNumber = 65535;

        /// <summary>
        /// Number of workers to work at the beginning
        /// </summary>
        private int InitialWorkerCount { get; set; }
        /// <summary>
        /// Current  number of workers
        /// </summary>
        private int CurrentWorkerCount { get; set; }

        /// <summary>
        /// Source object to create a CancellationToken and cancel the scan
        /// </summary>
        private CancellationTokenSource ScanCancellationTokenSource { get; set; }
        /// <summary>
        /// CancellationToken for workers
        /// </summary>
        private CancellationToken ScanCancellationToken { get; set; }

        /// <summary>
        /// Concurrent Queue to scan
        /// </summary>
        public ConcurrentQueue<ScannerQueueObject> ScannerQueues { get; set; }

        /// <summary>
        /// Active workers
        /// </summary>
        private List<Worker> Workers;

        /// <summary>
        /// This event is triggered when the port scan for an item is complete
        /// </summary>
        public event EventHandler<PortCheckCompletedEventArgs> OnPortCheckCompleted;
        /// <summary>
        /// This event is triggered when the scan started
        /// </summary>
        public event EventHandler<EventArgs> OnStarted;
        /// <summary>
        /// This event is triggered when the scan canceling
        /// </summary>
        public event EventHandler<EventArgs> OnCanceling;
        /// <summary>
        /// This event is triggered when the scan cancel action completed
        /// </summary>
        public event EventHandler<EventArgs> OnCanceled;
        /// <summary>
        /// This event is triggered when the scan finished
        /// </summary>
        public event EventHandler<EventArgs> OnFinished;

        /// <summary>
        /// contructer of scanner object
        /// initialWorkerCount: Number of workers to work at the beginning</param>
        /// </summary>
        public Scanner(int initialWorkerCount)
        {
            CurrentWorkerCount = 0;
            InitialWorkerCount = initialWorkerCount;

            Initialize();
        }

        /// <summary>
        /// Initialize private variables
        /// </summary>
        private void Initialize()
        {
            ScanCancellationTokenSource = new CancellationTokenSource();
            ScanCancellationToken = ScanCancellationTokenSource.Token;

            ScannerQueues = new ConcurrentQueue<ScannerQueueObject>();

            Workers = new List<Worker>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddresses">Scanning ip addresses to be added to the queue</param>
        /// <returns></returns>
        public async Task AddQueueAsync(IEnumerable<IPAddress> ipAddresses)
        {
            await Task.Run(() =>
            {
                foreach (var ipAddress in ipAddresses)
                {
                    for (int port = minPortNumber; port <= maxPortNumber; port++)
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

        /// <summary>
        /// Starts the scanning process
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Stops the scanning process
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            await Task.Run(() =>
            {
                OnCanceling?.Invoke(this, new EventArgs { });

                ScanCancellationTokenSource.Cancel();

                Task.WhenAll(Workers.Select(w => w.Task)).ContinueWith((t) =>
                {
                    OnCanceled?.Invoke(this, new EventArgs { });
                }).Wait();
            });
        }

        /// <summary>
        /// Increases the number of current worker by 1
        /// </summary>
        public void IncreaseWorker()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var task = CreateWorker(cancellationTokenSource.Token);

            var worker = new Worker(task, cancellationTokenSource);

            Workers.Add(worker);

            CurrentWorkerCount++;
        }
        /// <summary>
        /// Decreases the number of current worker by 1
        /// </summary>
        public void DecreaseWorker()
        {
            var lastworker = Workers.LastOrDefault();
            lastworker.CancellationTokenSource.Cancel();
            Workers.Remove(lastworker);

            CurrentWorkerCount--;
        }

        /// <summary>
        /// Sets the current number of employees according to the specified parameter value
        /// </summary>
        /// <param name="workerCount">Number of workers to be</param>
        /// <returns></returns>
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

        /// <summary>
        /// private scan method for a queue item
        /// </summary>
        /// <param name="scannerQueueObject"></param>
        private void Scan(ScannerQueueObject scannerQueueObject)
        {
            ScanCancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var tcpClient = new TcpClient())
                {
                    var timeout = TimeSpan.FromSeconds(10);

                    tcpClient.ReceiveTimeout = (int)timeout.TotalMilliseconds;
                    tcpClient.SendTimeout = (int)timeout.TotalMilliseconds;
                    var asyncResult = tcpClient.BeginConnect(scannerQueueObject.IPAddress.ToString(), scannerQueueObject.Port, null, null);
                    var waitHandle = asyncResult.AsyncWaitHandle;
                    try
                    {
                        if (!asyncResult.AsyncWaitHandle.WaitOne(timeout, false))
                        {
                            tcpClient.Close();
                            scannerQueueObject.Status = PortStatus.Closed;
                        }
                        else
                        {
                            scannerQueueObject.Status = tcpClient.Connected ? PortStatus.Open : PortStatus.Closed;
                        }

                        tcpClient.EndConnect(asyncResult);
                    }
                    finally
                    {
                        waitHandle.Close();
                    }
                }
            }
            catch
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

        /// <summary>
        /// Creates and start a worker 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                ScanCancellationTokenSource?.Dispose();

                if (Workers != null)
                    foreach (var worker in Workers)
                    {
                        worker.Task.Dispose();
                        worker.CancellationTokenSource?.Dispose();
                    }
            }

            ScannerQueues = null;
            Workers = null;

            _disposed = true;
        }
    }
}
