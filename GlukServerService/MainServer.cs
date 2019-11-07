using GlukModels;
using GlukServerService.network;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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

            });

            ProcessStartUpParameters();
            InitDatabaseBackup();

            LOG.Info("Main server initialized.");

        }
        
        public MainServer(ItemsReceived itemsReceived)
        {
            LOG.Info("Initializing main server...");
            _db = new Database();
            _netCommUdp = new NetCommUDP();
            _tcpServer = new NetCommTCP(itemsReceived);

            ProcessStartUpParameters();
            InitDatabaseBackup();

            LOG.Info("Main server initialized.");
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

        private void ProcessStartUpParameters()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistryKeys.RegistryPath);

            if (registryKey == null)
            {
                LOG.Warn("MIssing registry key HKEY_LOCAL_MACHINE\\" + RegistryKeys.RegistryPath);
                return;
            }

            string path = (string)registryKey?.GetValue(RegistryKeys.BackupPath);
            if (path != null)
            {
                string restoreOnStartup = (string)registryKey.GetValue(RegistryKeys.RestoreDbOnStartUp);
                if (restoreOnStartup != null)
                {
                    if (restoreOnStartup.ToLower().Trim().Equals("true"))
                    {
                        _db.RestoreBackup(path);
                    }
                }

                string testBackup = (string)registryKey.GetValue(RegistryKeys.TestBackup);
                if (testBackup != null)
                {
                    if (testBackup.ToLower().Trim().Equals("true"))
                    {
                        _db.MakeBackup(path);
                    }
                }
            }
            else
            {
                LOG.Info($"Missing backup path in local machine registry {RegistryKeys.RegistryPath}");
            }
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

        private void InitDatabaseBackup()
        {
            LOG.Info("Scheduling periodic database backup...");
            var dueTime = GetTimeSpanToMidnight();
            TimeSpan period = new TimeSpan(TimeSpan.TicksPerDay);

            _backupTimer = new Timer(x =>
            {
                BackupDatabase();
            }, null, dueTime, period);

        }

        private TimeSpan GetTimeSpanToMidnight()
        {
            TimeSpan timeSpan = new TimeSpan();
            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today;
            if (now.Equals(today))
            {
                timeSpan = TimeSpan.Zero;
            }
            else
            {
                timeSpan = today.AddDays(1) - now;
            }

            return timeSpan;
        }

        private void BackupDatabase()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistryKeys.RegistryPath);
            if (registryKey == null)
            {
                LOG.Warn($"Missing registry key for local machine: {RegistryKeys.RegistryPath}, database backup won't execute...");
                return;
            }
            string path = (string)registryKey.GetValue(RegistryKeys.BackupPath);
            if (path == null)
            {
                LOG.Warn($"Missing registry value \"{RegistryKeys.BackupPath}\", database backup won't execute...");
                return;
            }

            if (!CheckFilePath(path))
            {
                return;
            }

            _db.MakeBackup(path);
        }
    }
}
