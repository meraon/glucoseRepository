using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlukServerService.network;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            NetCommUDP udp = new NetCommUDP();
            udp.Listen();
        }
    }
}
