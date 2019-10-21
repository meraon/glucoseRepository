using GlukModels;
using GlukModels.DbQuery;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace GlukServerService
{
    public class Database
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string DateTimeFormat = "d. M. y H:mm:ss";



        private static Database _database;
        public string _baseConnectionString;
        public string _connectionString;

        private string _databaseName = "dia_data";
        private string _server = "localhost";
        private string _port = "3306";
        private string _user = "root";
        private string _password = "";


        public Database()
        {
            _baseConnectionString = "server=" + _server + ";port=" + _port + ";user=" + _user + ";password=" + _password;
            _connectionString = "server=" + _server + ";database=" + _databaseName + ";port=" + _port + ";user=" + _user + ";password=" + _password;
            InitDatabase();
        }


        public Database(string dbName)
        {
            _databaseName = dbName;
            _baseConnectionString = "server=" + _server + ";port=" + _port + ";user=" + _user + ";password=" + _password;
            _connectionString = "server=" + _server + ";database=" + _databaseName + ";port=" + _port + ";user=" + _user + ";password=" + _password;
            InitDatabase();
        }

        public void SaveGlucoses(List<Glucose> items)
        {
            LOG.Info("Saving glucoses");
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = GlucoseTable.GetInsertQuery(items);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        LOG.Error(e.ToString);
                    }
                }
            }
            LOG.Info("Glucoses saved");
        }

        public void SaveInsulins(List<Insulin> items)
        {
            LOG.Info("Saving Insulins");
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = InsulinTable.GetInsertQuery(items);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        LOG.Error(e.ToString);
                    }
                }
            }
            LOG.Info("Insulins saved");
        }

        public List<Glucose> GetGlucoses()
        {
            List<Glucose> glucoses = new List<Glucose>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM " + GlucoseTable.TableName;
                    var glucosesReader = command.ExecuteReader();
                    if (glucosesReader.HasRows)
                    {
                        while (glucosesReader.Read())
                        {
                            glucoses.Add(new Glucose(glucosesReader.GetInt32("_id"),
                                DateTimeStringToTimestamp(glucosesReader.GetString("timestamp")),
                                glucosesReader.GetFloat("value")
                                ));
                        }
                    }
                    else
                    {
                        LOG.Info("No rows found.");
                    }
                }
            }

            return glucoses;
        }

        public List<Insulin> GetInsulins()
        {
            List<Insulin> insulins = new List<Insulin>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM " + InsulinTable.TableName;
                    var insulinsReader = command.ExecuteReader();
                    if (insulinsReader.HasRows)
                    {
                        while (insulinsReader.Read())
                        {
                            insulins.Add(new Insulin(insulinsReader.GetInt32("_id"), 
                                DateTimeStringToTimestamp(insulinsReader.GetString("timestamp")),
                                insulinsReader.GetFloat("value"), 
                                insulinsReader.GetInt16("dayDosage") == 1
                            ));
                        }
                    }
                    else
                    {
                        LOG.Info("No rows found.");
                    }
                }
            }

            return insulins;
        }

        private void InitDatabase()
        {
            using (MySqlConnection connection = new MySqlConnection(_baseConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE DATABASE IF NOT EXISTS `" + _databaseName + "`;";
                    command.ExecuteNonQuery();
                }
            }

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = GlucoseTable.CreateTable;
                    command.ExecuteNonQuery();
                }

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = InsulinTable.CreateTable;
                    command.ExecuteNonQuery();
                }
            }

            

        }

        public void RestoreBackup(string file)
        {
            LOG.Info("Restoring backup...");
            FileInfo fInfo = new FileInfo(file);
            if (!fInfo.Exists)
            {
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ImportFromFile(file);
                        conn.Close();
                    }
                }
            }

            LOG.Info("Backup restored.");
        }

        public void MakeBackup(string file)
        {
            LOG.Info("Creating backup...");
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(file);
                        conn.Close();
                    }
                }
            }
            LOG.Info("Backup created.");
        }

        //TODO check if timestamp is correct && change methods location
        public static long DateTimeStringToTimestamp(string dt)
        {
            var dateTime = DateTime.Parse(dt);
            var totalSeconds = (long)dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return totalSeconds;
        }

        public static long DateTimeToTimestamp(DateTime dt)
        {
            var totalSeconds = (long)dt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return totalSeconds;
        }

        public static DateTime TimestampToDateTime(long timestamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(timestamp); 
            return dt;
        }
        
    }
}
