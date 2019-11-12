using NLog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GlukServerService.network
{
    public class NetCommUDP : INetComm
    {
        Logger LOG = LogManager.GetCurrentClassLogger();

        
        public static readonly string ExpectedMessage = "allo!";
        private readonly string _ipMulticast = "239.0.0.0";
        private readonly int _port = 11223;

        UdpClient server;

        public NetCommUDP()
        {
        }

        public NetCommUDP(int port)
        {
            _port = port;
        }

        public NetCommUDP(int port, string ipMulticast) : this(port)
        {
            _ipMulticast = ipMulticast;
        }

        public void Listen()
        {
            server = new UdpClient(_port);
            server.JoinMulticastGroup(IPAddress.Parse(_ipMulticast));
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
                byte[] message = socket.EndReceive(result, ref sourceEp);
                string msgString = Encoding.ASCII.GetString(message);

                if (IsValidMessage(msgString))
                {
                    string resonse = NetCommTCP.port.ToString();
                    socket.Send(Encoding.ASCII.GetBytes(resonse), resonse.Length, sourceEp);
                }
                else if (msgString.Trim().Equals("end"))
                {
                    socket.Close();
                    LOG.Warn("Closed UDP server.");
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
