using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace GlukServerService.network
{
    public class UdpServer : INetComm
    {
        Logger LOG = LogManager.GetCurrentClassLogger();

        private static readonly string IpMulticast = "239.0.0.0";
        public static readonly string ExpectedMessage = "allo!";
        public static readonly string ExpectedResponse = "nibb";
        int port = 11223;
        UdpClient server;
        
        public void Listen()
        {
            server = new UdpClient(port);
            server.JoinMulticastGroup(IPAddress.Parse(IpMulticast));
            server.BeginReceive(OnDataReceived, server);
        }

        public void Terminate()
        {
            server.Close();
        }

        private void OnDataReceived(IAsyncResult result)
        {
            try
            {
                if (!(result.AsyncState is UdpClient socket))
                {
                    LOG.Warn($"Expected AsyncState of type {typeof(UdpClient).Name}, ending UDP server.");
                    return;
                }
                IPEndPoint sourceEp = new IPEndPoint(0, 0);
                byte[] message = socket?.EndReceive(result, ref sourceEp);
                string msgString = Encoding.ASCII.GetString(message);

                if (IsValidMessage(msgString))
                {
                    socket.Send(Encoding.ASCII.GetBytes(ExpectedResponse), ExpectedResponse.Length, sourceEp);
                }
                else if (msgString.Trim().Equals("end"))
                {
                    socket.Close();
                    LOG.Warn("Closed UDP server");
                    return;
                }
                socket.BeginReceive(OnDataReceived, socket);
            }
            catch (Exception e)
            {

                LOG.Fatal(e.ToString);
            }
        }

        private bool IsValidMessage(string message)
        {
            return message.Trim().Equals(ExpectedMessage);
        }
    }
}
