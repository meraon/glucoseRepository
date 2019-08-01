using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GlukServerService.models;
using NLog;

namespace GlukServerService.network
{
    class NetCommTCP : INetComm
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        private NetCommTCP.INetworkListener networkHandler;
        private int port;
        private bool terminate = false;
        private bool running = false;

        public bool isRunning()
        {
            return this.running;
        }

        public NetCommTCP()
        {
            
        }

        public NetCommTCP(NetCommTCP.INetworkListener networkHandler)
        {
            

        }

        public NetCommTCP(NetCommTCP.INetworkListener networkHandler, int port)
        {
            

        }

        

        public void Listen()
        {
            

        }

        public void Terminate()
        {
           

        }

        public interface INetworkListener
        {
            void itemsReceived(List<EntryModel> var1);
        }
    }
}
