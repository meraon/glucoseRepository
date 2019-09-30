using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukServerService.models;
using MySqlX.XDevAPI;

namespace GlukServerService.database
{
    public class GlucoseTable
    {
        public static readonly string TableName = "glucose";
        public static readonly string CreateTable = "CREATE TABLE IF NOT EXISTS `glucose` " +
                                                    "(`_id` INT NOT NULL AUTO_INCREMENT," +
                                                    "`value` FLOAT NOT NULL DEFAULT '0'," +
                                                    "`timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                                                    "PRIMARY KEY (`_id`))";

        public static string GetInsertQuery(long timestamp, float value)
        {
            if(CultureInfo.CurrentCulture != Program.CULTURE) { }
            string INSERT_QUERY = "INSERT INTO `glucose` (`timestamp`,`value`) VALUES (FROM_UNIXTIME(%d * 0.001),%f)";
            return string.Format(INSERT_QUERY, timestamp, value);
        }

        public static string GetInsertQuery(List<Glucose> items)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            string insertQuery = "INSERT INTO `glucose` (`timestamp`,`value`) VALUES ";

            foreach (Glucose item in items)
            {
                insertQuery += $"(FROM_UNIXTIME({item.getTimestamp()} * 0.001),{item.getValue():N1}),";
            }

            return insertQuery.Substring(0, insertQuery.Length - 1);
        }
    }
}
