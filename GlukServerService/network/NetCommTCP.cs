using NLog;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GlukServerService.network
{
    public delegate void ItemsReceived(string json);

    public class NetCommTCP : INetComm
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        public static string ObjectAcceptedMessage = "OK\n";
        public static string ObjectRejectedMessage = "ERR\n";
        public static int port = 11123;

        private readonly ItemsReceived JsonReceived;

        private TcpListener server;
        private TcpClient client;
        private bool _terminate;
        private bool _isRunning;

        public bool IsRunning()
        {
            return _isRunning;
        }


        //TODO make TCP comm async
        public NetCommTCP()
        {
            
        }

        public NetCommTCP(ItemsReceived jsonReceived)
        {
            this.JsonReceived = jsonReceived;
        }

        public NetCommTCP(ItemsReceived jsonReceived, int port)
        {
            this.JsonReceived = jsonReceived;
            NetCommTCP.port = port;
        }

        

        public void Listen()
        {
            LOG.Info("TCP server started listening...");
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            _isRunning = true;
            while (!_terminate)
            {
                try
                {
                    LOG.Info("Waiting for client...");
                    client = server.AcceptTcpClient();
                    LOG.Info("Client accepted! " + client.Client.RemoteEndPoint);
                    StreamReader reader = new StreamReader(client.GetStream());
                    NetworkStream stream = client.GetStream();

                    string json = reader.ReadLine();
                    
                    if (!IsJsonValid(json))
                    {
                        //send response 
                        var responseRejected = Encoding.ASCII.GetBytes(ObjectRejectedMessage);
                        stream.Write(responseRejected, 0, responseRejected.Length);
                        stream.Flush();
                        continue;
                    }
                    //send response 
                    var responseOk = Encoding.ASCII.GetBytes(ObjectAcceptedMessage);
                    stream.Write(responseOk, 0, responseOk.Length);
                    stream.Flush();

                    //handle json
                    ItemsReceived(json);

                }
                catch (Exception e)
                {
                    _isRunning = false;
                    if (e is SocketException ex)
                    {
                        if (ex.ErrorCode == 10004)
                        {
                            LOG.Warn(ex.Message);
                        }
                        else
                        {
                            LOG.Error(ex.ToString);
                        }
                    }
                    else
                    {
                        LOG.Error(e.ToString);
                        throw;
                    }
                }
                finally
                {
                    client = null;
                }
            }
        }


        public void Terminate()
        {
            client?.Close();
            server.Stop();
            _terminate = true;
        }

        private void ItemsReceived(string json)
        {
            if(JsonReceived == null) return;
            var itemsReceivedThread = new Thread(() => JsonReceived(json));
            itemsReceivedThread.Start();
        }

        public static bool IsJsonValid(string json)
        {
            return json.StartsWith("[") && json.EndsWith("]");
        }
        
    }
}
