using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GlukServerService.network;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            NetCommUDP udp = new NetCommUDP();
            //Thread t = new Thread(() =>
            //{
            //    Thread.Sleep(10000);
            //    udp.Terminate();
            //});
            ////t.Start();
            udp.Listen();

            



        }
    }
}
