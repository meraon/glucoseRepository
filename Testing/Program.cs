using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukServerService;
using GlukServerService.models;
using GlukServerService.network;
using Newtonsoft.Json.Linq;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            MainServer mainServer = new MainServer();

            try
            {
                mainServer.Start();
                Console.ReadLine();
            }
            finally
            {
                mainServer.Terminate();
            }

            Console.ReadLine();


        }
    }
}
