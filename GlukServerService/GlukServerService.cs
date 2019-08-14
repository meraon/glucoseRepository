using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GlukServerService
{
    public partial class GlukServerService : ServiceBase
    {
        MainServer server = new MainServer();

        public GlukServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            server.Start();
        }

        protected override void OnStop()
        {
            server.Terminate();
        }
    }
}
