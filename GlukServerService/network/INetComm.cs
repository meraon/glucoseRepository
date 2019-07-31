using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlukServerService.network
{
    interface INetComm
    {
        void listen();
        void terminate();
    }
}
