using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlukServerService.models;
using MySqlX.XDevAPI;

namespace GlukServerService.database
{
    public class GlucoseTable
    {
        public static readonly string TABLE_NAME = "glucose";
        public static readonly string CREATE_TABLE = "CREATE TABLE `glucose` (`_id` INT NOT NULL AUTO_INCREMENT,`timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,`value` FLOAT NOT NULL DEFAULT '0',PRIMARY KEY (`_id`) )";

        public GlucoseTable()
        {
        }

        public static String GetInsertQuery(long timestamp, float value)
        {
            if(CultureInfo.CurrentCulture != Program.CULTURE) { }
            string INSERT_QUERY = "INSERT INTO `glucose` (`timestamp`,`value`) VALUES (FROM_UNIXTIME(%d * 0.001),%f)";
            return string.Format(INSERT_QUERY, timestamp, value);
        }

        public static string GetInsertQuery(List<Glucose> items)
        {
            string INSERT_QUERY = "INSERT INTO `glucose` (`timestamp`,`value`) VALUES ";

            //TODO build query

            return INSERT_QUERY.Substring(0, INSERT_QUERY.Length - 1);
        }
    }
}
