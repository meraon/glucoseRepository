using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace GlukServerService.network
{
    public class NetCommUDP : INetComm
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        private string ipMulticast;
        private int port;
        private MulticastSocket socket;
        private boolean terminate = false;

        public NetCommUDP()
        {
            this.ipMulticast = Network.IP_MULTICAST;
            this.port = Network.DEFAULT_PORT_UDP;
        }

        public NetCommUDP(int port)
        {
            this.ipMulticast = Network.IP_MULTICAST;
            this.port = port;
        }

        public NetCommUDP(String ipMulticast, int port)
        {
            this.ipMulticast = ipMulticast;
            this.port = port;
        }

        public NetCommUDP(Properties prop)
        {
            this.ipMulticast = prop.containsKey("ipMulticast") ? (String)prop.get("ipMulticast") : Network.IP_MULTICAST;
            this.port = prop.containsKey("portUDP") ? Integer.parseInt((String)prop.get("portUDP")) : Network.DEFAULT_PORT_UDP;
        }

        private boolean isValidMessage(String message)
        {
            return message.equals(Network.EXPECTED_MESSAGE);
        }

        private void sendResponse(String message, InetAddress address, int port)
        {
            try
            {
                byte[] buf = new byte[100];
                DatagramSocket socket = new DatagramSocket();
                buf = message.getBytes();
                DatagramPacket packet = new DatagramPacket(buf, buf.length, address, port);
                socket.send(packet);
                socket.close();
            }
            catch (SocketException var7)
            {
                LOG.severe(var7.toString());
            }
            catch (UnknownHostException var8)
            {
                LOG.severe(var8.toString());
            }
            catch (IOException var9)
            {
                LOG.severe(var9.toString());
            }

        }

        public void listen()
        {
            this.terminate = false;
            byte[] buf = new byte[100];

            try
            {
                this.socket = new MulticastSocket(this.port);
                InetAddress group = InetAddress.getByName(this.ipMulticast);
                this.socket.joinGroup(group);

                String received;
                do
                {
                    DatagramPacket packet = new DatagramPacket(buf, buf.length);
                    this.socket.receive(packet);
                    received = new String(packet.getData(), 0, packet.getLength());
                    if (this.isValidMessage(received))
                    {
                        this.sendResponse(Network.EXPECTED_RESPONSE, packet.getAddress(), packet.getPort());
                    }
                } while (!"end".equals(received));

                this.socket.leaveGroup(group);
                this.socket.close();
            }
            catch (UnknownHostException var5)
            {
                LOG.severe(var5.toString());
            }
            catch (IOException var6)
            {
                if (var6.getMessage().equals("socket closed") && this.terminate)
                {
                    LOG.severe("Socket closed by user...");
                }

                LOG.severe(var6.toString());
            }

        }

        public void terminate()
        {
            this.terminate = true;
            this.socket.close();
        }
    }
}
