using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukModels;
using GlukModels.DbQuery;
using GlukServerService;
using GlukServerService.network;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace Testing
{
    class Program
    {

        private static DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private static string _dbName = "glukdb_test";

        
        static void Main(string[] args)
        {

            //Database db = new Database("glukdb_test");
            //List<Glucose> items = new List<Glucose>();
            //for (int i = 0; i < 100; i++)
            //{
            //    DateTime dt = DateTime.Today.AddDays(i / 4);
            //    Glucose item = new Glucose();

            //    if (i % 4 == 0)
            //    {
            //        dt = dt.AddHours(21);
            //    }
            //    else if (i % 3 == 0)
            //    {
            //        dt = dt.AddHours(17);
            //    }
            //    else if (i % 2 == 0)
            //    {
            //        dt = dt.AddHours(12);
            //    }
            //    else
            //    {
            //        dt = dt.AddHours(7);
            //    }
            //    item.setTimestamp((long)(dt - Jan1st1970).TotalMilliseconds);
            //    item.setValue( (float)(5 + Math.Abs(5 *Math.Sin(i))) );
            //    items.Add(item);
            //}

            //db.SaveGlucoses(items);

            List<Glucose> glucoses = new List<Glucose>();
            List<Insulin> insulins = new List<Insulin>();
            for (int i = 0; i < 10; i++)
            {
                DateTime dt = DateTime.Now;
                glucoses.Add(new Glucose(i, dt.AddDays(i).Ticks, (float)Math.Abs(5 + 5 * Math.Sin(i))));
                insulins.Add(new Insulin(i, dt.AddDays(i).Ticks, (float)Math.Abs(5 + 10 * Math.Sin(i)), i % 4 == 0));
            }

            string s1 = GlucoseTable.GetDeleteQuery(glucoses[0]);
            string s2 = GlucoseTable.GetDeleteQuery(glucoses);


            try
            {
                string s3 = GlucoseTable.GetDeleteQuery(new List<Glucose>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                List<Glucose> list = null;
                string s4 = GlucoseTable.GetDeleteQuery(list);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Database db = new Database("glukdb_test");

            //using (MySqlConnection connection = new MySqlConnection(db._connectionString))
            //{
            //    connection.Open();
            //    using (MySqlCommand command = connection.CreateCommand())
            //    {
            //        command.CommandText = GlucoseTable.GetDeleteQuery(glucoses);
            //        command.ExecuteNonQuery();
            //    }
            //}

            //InitDbWithDummyData(_dbName);



            Console.ReadLine();


        }

        private static void InitDbWithDummyData(string dbName)
        {
            Database db = new Database(dbName);
            db.DropTables();
            db.InitDatabase();
            List<Glucose> glucoses = new List<Glucose>();
            List<Insulin> insulins= new List<Insulin>();
            for (int i = 0; i < 100; i++)
            {
                DateTime dt = DateTime.Today.AddDays(i / 4);
                Glucose item = new Glucose();
                Insulin insulin = new Insulin();

                if (i % 4 == 0)
                {
                    insulin.setIsDayDosage(true);
                    insulin.setValue(16);
                    dt = dt.AddHours(21);
                }
                else if (i % 3 == 0)
                {
                    insulin.setIsDayDosage(false);
                    insulin.setValue(10);
                    dt = dt.AddHours(17);
                }
                else if (i % 2 == 0)
                {
                    insulin.setIsDayDosage(false);
                    insulin.setValue(11);
                    dt = dt.AddHours(12);
                }
                else
                {
                    insulin.setIsDayDosage(false);
                    insulin.setValue(12);
                    dt = dt.AddHours(7);
                }
                insulin.setTimestamp((long)(dt - Jan1st1970).TotalMilliseconds);
                item.setTimestamp((long)(dt - Jan1st1970).TotalMilliseconds);
                item.setValue((float)(5 + Math.Abs(5 * Math.Sin(i))));
                glucoses.Add(item);
                insulins.Add(insulin);
            }

            db.SaveGlucoses(glucoses);
            db.SaveInsulins(insulins);
        }
    }
}
