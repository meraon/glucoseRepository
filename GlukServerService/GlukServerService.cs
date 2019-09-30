using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using NLog;

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
            server = new MainServer();
            server.Start();
            LOG.Info("Main server started");
        }

        protected override void OnStop()
        {
            LOG.Info("Terminating main server...");
            server.Terminate();
            LOG.Info("Main server terminated");
        }
        
    }
}
