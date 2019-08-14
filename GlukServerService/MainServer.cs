using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukServerService.models;
using GlukServerService.network;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using NLog.Fluent;

namespace GlukServerService
{
    public class MainServer
    {
        private NetCommUDP _udpServer;
        private NetCommTCP _tcpServer;

        public MainServer()
        {
            _udpServer = new NetCommUDP();
            _tcpServer = new NetCommTCP(json =>
            {
                //TODO deserialize json
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
                
            } );
        }

        public MainServer(ItemsReceived itemsReceived)
        {
            _udpServer = new NetCommUDP();
            _tcpServer = new NetCommTCP(itemsReceived);
        }


        public void Start()
        {
            var tcpServerThread = new Thread(() => _tcpServer.Listen());
            var udpServerThread = new Thread(() => _udpServer.Listen());
            tcpServerThread.Start();
            udpServerThread.Start();
        }

        public void Terminate()
        {
            _tcpServer.Terminate();
            _udpServer.Terminate();
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
