using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace GlukServerService
{
    public partial class GlukServerService : ServiceBase
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        MainServer server = new MainServer();

        public GlukServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LOG.Info("Starting main server...");
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
