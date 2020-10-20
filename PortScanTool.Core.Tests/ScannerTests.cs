using NetTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanTool.Core.Tests
{
    [TestFixture]
    public class ScannerTests
    {
        private Scanner _scanner;

        [OneTimeSetUp]
        public void Init()
        {
            _scanner = new Scanner(1);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _scanner.Dispose();
            _scanner = null;

        }

        [Test]
        [Order(1)]
        public void Is_Setup_Correct()
        {
            Assert.IsNotNull(_scanner, "Setup not initialize");
        }

        [Test]
        [Order(2)]
        [TestCaseSource("Is_ScannerAddQueue_Run_Correctly_TestCaseSource")]
        public void Is_ScannerAddQueue_Run_Correctly(IPAddressRange ipAddressRange)
        {
            Assert.DoesNotThrowAsync(async () => { await _scanner.AddQueueAsync(ipAddressRange); });
            Assert.IsNotNull(_scanner.ScannerQueues);
            CollectionAssert.IsNotEmpty(_scanner.ScannerQueues);

        }
        public static IEnumerable<IPAddressRange> Is_ScannerAddQueue_Run_Correctly_TestCaseSource()
        {
            yield return IPAddressRange.Parse("127.0.0.1-127.0.0.1");
        }

        [Test]
        [Order(3)]
        public void Is_Scanner_StartAsync_Run_Correctly()
        {
            Assert.DoesNotThrowAsync(async () => { await _scanner.StartAsync(); });
        }

        [Test]
        [Order(4)]
        public void Is_Scanner_Queue_Runout()
        {
            var qcountbefore = _scanner.ScannerQueues.Count;
            Thread.Sleep(TimeSpan.FromSeconds(10));
            var qcountafter = _scanner.ScannerQueues.Count;
            Assert.Less(qcountafter, qcountbefore);
        }

        [Test]
        [Order(5)]
        [TestCase(2)]
        public void Is_Scanner_ChangeWorkerCountAsync_Run_Correctly(int workerCount)
        {
            Assert.DoesNotThrowAsync(async () => { await _scanner.ChangeWorkerCountAsync(workerCount); });
            Assert.AreEqual(workerCount, _scanner.CurrentWorkerCount);
        }

        [Test]
        [Order(6)]
        public void Is_Scanner_Stop_Run_Correctly()
        {
            Assert.DoesNotThrowAsync(async () => { await _scanner.StopAsync(); });
        }
    }
}
