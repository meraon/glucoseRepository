using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlukServerService.database;
using GlukServerService.models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using NLog;

namespace GlukServerService
{
    public class Database
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private static Database _database;
        public string _baseConnectionString;
        public string _connectionString;

        private string _databaseName = "dia_data";
        private string _server = "localhost";
        private string _port = "3306";
        private string _user = "root";
        private string _password = "";

        public static Database getInstance()
        {
            return _database ?? (_database = new Database());
        }

        public void SaveGlucoses(List<Glucose> items)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = GlucoseTable.GetInsertQuery(items);
                    command.ExecuteNonQuery();
                }
            }
            
        }

        public void SaveInsulins(List<Insulin> items)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = InsulinTable.GetInsertQuery(items);
                    command.ExecuteNonQuery();
                }
            }
        }

        private Database()
        {
            _baseConnectionString = "server=" + _server + ";port=" + _port + ";user=" + _user + ";password=" + _password;
            _connectionString = "server=" + _server + ";database=" + _databaseName + ";port=" + _port + ";user=" + _user + ";password=" + _password;
            InitDatabase();
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
                    command.CommandText = GlucoseTable.CREATE_TABLE;
                    command.ExecuteNonQuery();
                }

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = InsulinTable.CREATE_TABLE;
                    command.ExecuteNonQuery();
                }
            }

            

        }

        
    }
}
