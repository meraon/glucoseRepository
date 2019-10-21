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

        private static DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        static void Main(string[] args)
        {

            Database db = new Database("glukdb_test");
            List<Glucose> items = new List<Glucose>();
            for (int i = 0; i < 100; i++)
            {
                DateTime dt = DateTime.Today.AddDays(i / 4);
                Glucose item = new Glucose();
                
                if (i % 4 == 0)
                {
                    dt = dt.AddHours(21);
                }
                else if (i % 3 == 0)
                {
                    dt = dt.AddHours(17);
                }
                else if (i % 2 == 0)
                {
                    dt = dt.AddHours(12);
                }
                else
                {
                    dt = dt.AddHours(7);
                }
                item.setTimestamp((long)(dt - Jan1st1970).TotalMilliseconds);
                item.setValue( (float)(5 + Math.Abs(5 *Math.Sin(i))) );
                items.Add(item);
            }

            db.SaveGlucoses(items);

            Console.ReadLine();


        }
    }
}
