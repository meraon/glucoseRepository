using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GlukServerService
{
    public class Database
    {
        private static Database _database;

        private string _connectionString;
        private MySqlConnection _connection;

        private string _databaseName = "test";
        private string _server = "localhost";
        private string _port = "3306";
        private string _user = "root";
        private string _password = "";

        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        private Database()
        {
            _connectionString = "server=" + _server + ";database=" + _databaseName + ";port=" + _port + ";user=" + _user + ";password=" + _password;
            _connection = new MySqlConnection(_connectionString);
        }

        public static Database getInstance()
        {
            return _database ?? (_database = new Database());
        }

        
    }
}
