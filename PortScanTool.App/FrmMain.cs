using NetTools;
using NLog;
using PortScanTool.App.Models;
using PortScanTool.Core;
using PortScanTool.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortScanTool.App
{
    public partial class FrmMain : Form
    {
        private readonly SynchronizationContext synchronizationContext;

        private Scanner Scanner;

        private List<ScanResultItem> ScanResultItems;

        private static Logger logger;

        public FrmMain()
        {
            InitializeComponent();

            synchronizationContext = SynchronizationContext.Current;

            InitializeForm();
        }

        private void InitializeForm()
        {
            TbParallelTaskCount.Minimum = 1;
            TbParallelTaskCount.Maximum = Environment.ProcessorCount;
            TbParallelTaskCount.Value = 1;
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (logger == null) logger = LogManager.GetCurrentClassLogger();
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                logger.Info("Scan starting");

                if (!IPAddressRange.TryParse(TxtIPRange.Text.Replace(",", ".").Replace(" ", ""), out IPAddressRange ipAddressRange))
                {
                    MessageBox.Show("IP Range not valid!", "Input Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    logger.Info("IP Range validation failed. Scan aborted");

                    return;
                }

                BtnStart.Enabled = false;

                logger.Info("Scan initializing");
                LblStatusText.Text = "Scan Initializing";

                ScanResultItems = new List<ScanResultItem>();
                DgwScanResults.DataSource = typeof(ScanResultItem);
                DgwScanResults.DataSource = ScanResultItems;


                Scanner = new Scanner(TbParallelTaskCount.Value);

                Scanner.OnStarted += Scanner_OnStarted;
                Scanner.OnCanceled += Scanner_OnCanceled;
                Scanner.OnFinished += Scanner_OnFinished;
                Scanner.OnPortCheckCompleted += Scanner_OnPortCheckCompleted;

                logger.Info("Scan queue populating");

                await Scanner.AddQueueAsync(ipAddressRange.AsEnumerable());

                logger.Info("Scan start requesting");

                await Scanner.StartAsync();
            }
            catch (AggregateException)
            {
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;
            }
            catch (OperationCanceledException)
            {
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async void BtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                logger.Info("Scan stop requesting");

                await Scanner.StopAsync();

                Scanner = null;
            }
            catch (OperationCanceledException)
            {

            }

            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async void TbParallelTaskCount_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (Scanner != null)
                {
                    logger.Info("Scan worker count change requested for {0}", TbParallelTaskCount.Value);

                    await Scanner.ChangeWorkerCountAsync(TbParallelTaskCount.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void Scanner_OnPortCheckCompleted(object sender, PortCheckCompletedEventArgs e)
        {
            try
            {
                logger.Debug("Scan item check completed, {0}:{1} => {2}", e.IPAddress, e.Port, e.Status);

                if (e.Status == PortStatus.Open)
                {
                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {

                        ScanResultItems.Add(new ScanResultItem
                        {
                            IP_Address = e.IPAddress.ToString(),
                            Port = e.Port,
                            Status = e.Status
                        });

                        DgwScanResults.DataSource = typeof(ScanResultItem);
                        DgwScanResults.DataSource = ScanResultItems;

                    }), null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void Scanner_OnFinished(object sender, EventArgs e)
        {
            logger.Info("Scan finished");

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;
                LblStatusText.Text = "Scan completed";
            }), null);
        }

        private void Scanner_OnCanceled(object sender, EventArgs e)
        {
            logger.Info("Scan canceled");

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;
                LblStatusText.Text = "Scan Canceled";
            }), null);
        }

        private void Scanner_OnStarted(object sender, EventArgs e)
        {
            logger.Info("Scan started");

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                LblStatusText.Text = "Scanning";
                BtnStop.Enabled = true;
            }), null);
        }

    }
}
