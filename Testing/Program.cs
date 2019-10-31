using GlukModels;
using GlukModels.DbQuery;
using GlukServerService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Utilities;

namespace Testing
{
    class Program
    {

        private static DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private static string _dbName = "glukdb_test";
        private static Dictionary<MyType, MyOtherType> mapDictionary = new Dictionary<MyType, MyOtherType>();


        static void Main(string[] args)
        {

            
            List<MyType> myTypes = new List<MyType>
            {
                new MyType("name1"),
                new MyType("name2"),
                new MyType("name3"),
            };

            List<MyOtherType> myOtherTypes = new List<MyOtherType>
            {
                new MyOtherType(1),
                new MyOtherType(2),
                new MyOtherType(3)
            };

            for (int i = 0; i < 3; i++)
            {
                mapDictionary.Add(myTypes[i], myOtherTypes[i]);
            }
            


            while (true)
            {
                Console.WriteLine();
                foreach (var key in mapDictionary.Keys)
                {
                    Console.WriteLine($"key: {key} , value: {mapDictionary[key]}");
                }

                Console.WriteLine("1 - add new\n2 - update value\n");
                var choice = Console.ReadLine();
                if (int.TryParse(choice, out int result))
                {
                    switch (result)
                    {
                        case 1:
                            if (!AddNewKeyPair())
                            {
                                return;
                            }
                            break;
                        case 2:
                            if (!UpdateValue())
                            {
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }
                
               
            }



            //string pass = "my pasqwqwERQWeqsword";
            //string encrypted = Connection.Encrypt(pass);
            //string decrypted = Connection.Decrypt(encrypted);

            //Console.WriteLine("pass: " + pass + Environment.NewLine
            //                  + "encrypted: " + encrypted + Environment.NewLine
            //                  + "decrypted: " + decrypted + Environment.NewLine
            //                  + (pass == decrypted));
            //byte[] salt;
            //new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            //Console.WriteLine(Convert.ToBase64String(salt));

            //Dictionary<string, string> props = new Dictionary<string, string>();
            //props.Add("dbName", "dia_data");
            //props.Add("server", "localhost");
            //props.Add("port", "3306");
            //props.Add("user", "root");
            //props.Add("password", "nigger");
            //Connection.SaveConnectionProperties(props, Directory.GetCurrentDirectory());
            //var newProps = Connection.GetConnectionProperties(Directory.GetCurrentDirectory());
            //Console.WriteLine(props.Equals(newProps));




            //List<Glucose> glucoses = new List<Glucose>();
            //List<Insulin> insulins = new List<Insulin>();
            //for (int i = 0; i < 10; i++)
            //{
            //    DateTime dt = DateTime.Now;
            //    glucoses.Add(new Glucose(i, dt.AddDays(i).Ticks, (float)Math.Abs(5 + 5 * Math.Sin(i))));
            //    insulins.Add(new Insulin(i, dt.AddDays(i).Ticks, (float)Math.Abs(5 + 10 * Math.Sin(i)), i % 4 == 0));
            //}



            //string s1 = GlucoseTable.GetUpdateQuery(glucoses[0]);
            //string s2 = GlucoseTable.GetDeleteQuery(glucoses);


            //try
            //{
            //    string s3 = GlucoseTable.GetDeleteQuery(new List<Glucose>());
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            //try
            //{
            //    List<Glucose> list = null;
            //    string s4 = GlucoseTable.GetDeleteQuery(list);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

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

        private static bool UpdateValue()
        {
            Console.Write("set key: ");
            var keyInput = Console.ReadLine();

            var keyType = new MyType(keyInput);
            var value = mapDictionary[keyType];
            Console.WriteLine($"key: {keyType} , value: {value}");
            Console.WriteLine("new value: ");
            var newValueStr = Console.ReadLine();
            if (keyInput.Equals("end") || newValueStr.Equals("end"))
            {
                return false;
            }

            if (int.TryParse(newValueStr, out int nval))
            {
                mapDictionary[keyType] = new MyOtherType(nval);
            }
            else
            {
                Console.WriteLine("Invalid value");
            }

            Console.WriteLine();
            return true;
        }

        private static bool AddNewKeyPair()
        {
            Console.WriteLine();
            Console.Write("key: ");
            var keyStr = Console.ReadLine();

            if (keyStr == null) return false;
            if (keyStr.Equals("end"))
            {
                return false;
            }

            Console.Write("value: ");
            var valueStr = Console.ReadLine();

            if (valueStr == null) return false;
            if (valueStr.Equals("end"))
            {
                return false;
            }
            
            if (int.TryParse(valueStr, out int val))
            {
                mapDictionary.Add(new MyType(keyStr), new MyOtherType(val));
            }
            else
            {
                Console.WriteLine("Cant add pair");
            }

            Console.WriteLine();
            return true;
        }

        private static void InitDbWithDummyData(string dbName)
        {
            Database db = new Database(dbName);
            db.DropTables();
            db.InitDatabase();
            List<Glucose> glucoses = new List<Glucose>();
            List<Insulin> insulins = new List<Insulin>();
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

    public class MyType
    {
        public string Name { get; set; }

        public MyType(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is MyType str)
            {
                return Name.Equals(str.Name);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (7 * hash) + Name.GetHashCode();
            return hash;
        }
    }

    public class MyOtherType
    {
        public int Number{ get; set; }

        public MyOtherType(int number)
        {
            Number = number;
        }

        public override string ToString()
        {
            return Number.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is MyOtherType str)
            {
                return Number.Equals(str.Number);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (7 * hash) + Number.GetHashCode();
            return hash;
        }
    }
}
