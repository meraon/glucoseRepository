using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukServerService;
using GlukServerService.network;

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


        }
    }
}
