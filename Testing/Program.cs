using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukModels;
using GlukServerService;
using GlukServerService.network;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            Database db = new Database();
            var glucoses = db.GetGlucoses();

            foreach (Glucose glucose in glucoses)
            {
                Console.WriteLine(Database.TimestampToDateTime(glucose.getTimestamp()).ToString("G"));
            }

            Console.ReadLine();


        }
    }
}
