using System;
using System.Collections.Generic;
using System.IO;
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
        private NetCommTCP _tcpServer;
        private NetCommUDP _netCommUdp;
        private Database _db;

        private Timer _backupTimer;


        public MainServer()
        {
            LOG.Info("Initializing main server...");
            _db = new Database();
            _netCommUdp = new NetCommUDP();
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
            _db = new Database();
            _netCommUdp = new NetCommUDP();
            _tcpServer = new NetCommTCP(itemsReceived);
            LOG.Info("Main server initialized.");
        }

        public MainServer(string[] args) : this()
        {
            if(args.Length > 0) HandleArgs(args);
        }

        public MainServer(ItemsReceived itemsReceived, string[] args) : this(itemsReceived)
        {
            if (args.Length > 0) HandleArgs(args);
        }

        public void Start()
        {
            _netCommUdp.Listen();
            var tcpServerThread = new Thread(() => _tcpServer.Listen());
            tcpServerThread.Start();
        }

        public void Terminate()
        {
            LOG.Info("Terminating server...");

            _backupTimer?.Dispose();
            _tcpServer?.Terminate();
            _netCommUdp?.Terminate();
        }

        public void SaveGlucoses(List<Glucose> items)
        {
            if (items.Count < 1) return;
            Log.Info("Saving " + items.Count + " glucose items.");
            _db.SaveGlucoses(items);
        }

        public void SaveInsulins(List<Insulin> items)
        {
            if (items.Count < 1) return;
            Log.Info("Saving " + items.Count + " insulin items.");
            _db.SaveInsulins(items);
        }

        private void HandleArgs(string[] args)
        {
            //check for correct filepath
            if (!CheckFilePath(args[0])) return;

            //check for second argument(restore backup at start)
            if (args.Length > 1)
            {
                if (args[1].Trim().ToLower().Equals("true"))
                {
                    _db.RestoreBackup(args[0]);
                }
                else if (args[1].Trim().ToLower().Equals("test_backup"))
                {
                    _db.MakeBackup(args[0]);
                    return;
                }
            }

            DatabaseBackup(args[0]);
        }

        public static bool CheckFilePath(string path)
        {
            try
            {
                FileInfo fInfo = new FileInfo(path); //throws exception if path has invalid characters, is empty or null
                return true;
            }
            catch (Exception e)
            {
                LOG.Warn("Invalid file path format!");
                return false;
            }
        }

        private void DatabaseBackup(string path)
        {
            LOG.Info("Scheduling periodic database backup...");
            
            //calculate time to execute every midnight
            TimeSpan dueTime = new TimeSpan();
            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today;
            if (now.Equals(today))
            {
                dueTime = TimeSpan.Zero;
            }
            else
            {
                dueTime = today.AddDays(1) - now;
            }
            TimeSpan period = new TimeSpan(TimeSpan.TicksPerDay);



            //execution timer
            _backupTimer = new Timer(x =>
            {
                _db.MakeBackup(path);
            }, null, dueTime, period);

        }
    }
}
