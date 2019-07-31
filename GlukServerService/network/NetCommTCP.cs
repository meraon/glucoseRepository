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
        public ServerSocket serverSocket;
        private int port;
        private bool terminate = false;
        private bool running = false;

        public bool isRunning()
        {
            return this.running;
        }

        public NetCommTCP()
        {
            this.port = Network.DEFAULT_PORT;
        }

        public NetCommTCP(NetCommTCP.INetworkListener networkHandler)
        {
            this.networkHandler = networkHandler;
            this.port = Network.DEFAULT_PORT;

            try
            {
                this.serverSocket = new ServerSocket(this.port);
            }
            catch (IOException var3)
            {
                var3.printStackTrace();
            }

        }

        public NetCommTCP(NetCommTCP.INetworkListener networkHandler, int port)
        {
            this.networkHandler = networkHandler;
            this.port = port;

            try
            {
                this.serverSocket = new ServerSocket(port);
            }
            catch (IOException var4)
            {
                var4.printStackTrace();
            }

        }

        public NetCommTCP(Properties prop)
        {
            this.port = prop.containsKey("portTCP") ? Integer.parseInt((String)prop.get("portTCP")) : Network.DEFAULT_PORT;

            try
            {
                this.serverSocket = new ServerSocket(this.port);
            }
            catch (IOException var3)
            {
                var3.printStackTrace();
            }

        }

        public NetCommTCP(NetCommTCP.INetworkListener networkHandler, Properties prop)
        {
            this.port = prop.containsKey("portTCP") ? Integer.parseInt((String)prop.get("portTCP")) : Network.DEFAULT_PORT;
            this.networkHandler = networkHandler;

            try
            {
                this.serverSocket = new ServerSocket(this.port);
            }
            catch (IOException var4)
            {
                var4.printStackTrace();
            }

        }

        public void listen()
        {
            try
            {
                LOG.info("Starting to listen...");
                this.running = true;

                while (true)
                {
                    Socket socket = this.serverSocket.accept();
                    LOG.info("Socket accepted !");
                    ObjectInputStream inputStream = new ObjectInputStream(socket.getInputStream());
                    Object received = inputStream.readObject();
                    ArrayList<Entry> items = (ArrayList)received;
                    BufferedWriter outputStream = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
                    if (items == null)
                    {
                        LOG.severe("Invalid object received!");
                        outputStream.write(Network.OBJECT_REJECTED_MESSAGE + "\n");
                    }
                    else
                    {
                        if (this.networkHandler != null)
                        {
                            this.networkHandler.itemsReceived(items);
                        }

                        outputStream.write(Network.OBJECT_ACCEPTED_MESSAGE + "\n");
                        LOG.info("Items OK");
                    }

                    outputStream.flush();
                }
            }
            catch (IOException var10)
            {
                if (var10.getMessage().equals("socket closed") && this.terminate)
                {
                    LOG.severe("Socket closed by user...");
                }

                LOG.severe(var10.toString());
            }
            catch (ClassNotFoundException var11)
            {
                LOG.severe(var11.toString());
            }
            finally
            {
                this.running = false;
            }

        }

        public void Terminate()
        {
            try
            {
                this.terminate = true;
                if (this.serverSocket != null && !this.serverSocket.isClosed())
                {
                    this.serverSocket.close();
                }
            }
            catch (IOException var2)
            {
                LOG.Fatal(var2.ToString());
            }

        }

        public interface INetworkListener
        {
            void itemsReceived(List<EntryModel> var1);
        }
    }
}
