using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace GlukServerService.network
{
    public class NetCommUDP : INetComm
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private static readonly string IpMulticast = "239.0.0.0";
        public static readonly string ExpectedMessage = "allo!";
        public static readonly string ExpectedResponse = "nibb";
        public static readonly int BufferSize = 1024;
        
        private readonly IPAddress _multicastIp = IPAddress.Parse(IpMulticast);
        private readonly int _port = 11223;
        private UdpClient server;
        private bool _terminate = false;

        public void Listen()
        {
            LOG.Info("UDP server started listening...");
            server = new UdpClient(_port);
            server.JoinMulticastGroup(IPAddress.Parse(IpMulticast));
            Receive();
        }

        public void Terminate()
        {
            this._terminate = true;
            server.Close();
        }


        private bool isValidMessage(string message)
        {
            return message.Equals(ExpectedMessage);
        }

        private void sendResponse(IPEndPoint ep)
        {
            var message = Encoding.ASCII.GetBytes(ExpectedResponse);
            server.Send(message, message.Length, ep);

        }

        private void Receive()
        {
            while (!_terminate)
            {
                try
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                    LOG.Info("Waiting for client...");
                    var bytes = server.Receive(ref ep);
                    LOG.Info("Client accepted! " + ep.Address + ":" + ep.Port);
                    string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    LOG.Info("Client message: " + message);
                    if (isValidMessage(message))
                    {
                        sendResponse(ep);
                    }

                }
                catch (SocketException ex)
                {
                    int code = ex.ErrorCode;
                    if (code == 10004)
                    {
                        LOG.Warn(ex.Message);
                    }
                    else
                    {
                        LOG.Fatal(ex.ToString());
                    }
                }
            }
        }

        
    }
}
