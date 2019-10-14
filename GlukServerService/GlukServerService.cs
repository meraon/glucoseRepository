using Microsoft.Win32;
using NLog;
using System.Collections.Generic;
using System.ServiceProcess;

namespace GlukServerService
{
    public partial class GlukServerService : ServiceBase
    {
        public static Dictionary<string, string> RegistryKeyDictionary = new Dictionary<string, string>
        {
            { RegistryKeys.BackupPath, @"D:\dia_data_backup.sql" },
            { RegistryKeys.RestoreDbOnStartUp, @"false" },
            { RegistryKeys.TestBackup, @"false" }
        };

        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        
        MainServer server;

        public GlukServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LOG.Info("Starting main server...");
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(RegistryKeys.RegistryPath);
            if (regKey == null)
            {
                CreateDefaultRegKeys();
            }

            server = new MainServer();
            server.Start();
            LOG.Info("Main server started");
        }

        private void CreateDefaultRegKeys()
        {
            RegistryKey regKey = Registry.LocalMachine.CreateSubKey(RegistryKeys.RegistryPath, true);
            foreach (var keyValue in RegistryKeyDictionary)
            {
                regKey.SetValue(keyValue.Key, keyValue.Value);
            }
        }

        protected override void OnStop()
        {
            LOG.Info("Terminating main server...");
            server.Terminate();
            LOG.Info("Main server terminated");
        }
        
    }
}
