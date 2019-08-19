using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukServerService.models;
using GlukServerService.network;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Fluent;

namespace GlukServerService
{
    public class MainServer
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private NetCommUDP _udpServer;
        private NetCommTCP _tcpServer;
        private UdpServer udpServer;



        public MainServer()
        {
            LOG.Info("Initializing main server...");

            udpServer = new UdpServer();
            _tcpServer = new NetCommTCP(json =>

            {
                //TODO deserialize json
                try
                {
                    if (json.Contains("isDayDosage"))
                    {
                        var jArray = JArray.Parse(json);
                        var items = jArray.ToObject<List<Insulin>>();
                        SaveInsulins(items);
                    }
                    else
                    {
                        var jArray = JArray.Parse(json);
                        var items = jArray.ToObject<List<Glucose>>();
                        SaveGlucoses(items);
                    }
                }
                catch (JsonReaderException ex)
                {
                    LOG.Warn(
                        $"Error encountered while parsing received JSON on line {ex.LineNumber} position {ex.LinePosition}\nJSON: {json}");
                }
                
            } );

            LOG.Info("Main server initialized.");

        }

        public MainServer(ItemsReceived itemsReceived)
        {
            LOG.Info("Initializing main server...");
            udpServer = new UdpServer();
            _tcpServer = new NetCommTCP(itemsReceived);
            LOG.Info("Main server initialized.");
        }


        public void Start()
        {
            udpServer.Listen();
            var tcpServerThread = new Thread(() => _tcpServer.Listen());
            tcpServerThread.Start();
        }

        public void Terminate()
        {
            _tcpServer.Terminate();
            udpServer.Terminate();
        }

        public void SaveGlucoses(List<Glucose> items)
        {
            if (items.Count < 1) return;
            Log.Info("Saving " + items.Count + " glucose items.");
            var db = Database.getInstance();
            db.SaveGlucoses(items);
        }

        public void SaveInsulins(List<Insulin> items)
        {
            if (items.Count < 1) return;
            Log.Info("Saving " + items.Count + " insulin items.");
            var db = Database.getInstance();
            db.SaveInsulins(items);
        }
    }
}
