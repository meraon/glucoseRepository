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
    public class InsulinTable
    {
        public static readonly string TableName = "insulin";
        public static readonly string CreateTable = "CREATE TABLE IF NOT EXISTS `insulin` " +
                                                    "(`_id` INT NOT NULL AUTO_INCREMENT," +
                                                    "`value` FLOAT NOT NULL DEFAULT '0'," +
                                                    "`dayDosage` TINYINT(1) NOT NULL DEFAULT '1'," +
                                                    "`timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                                                    "PRIMARY KEY (`_id`))";

        public static string GetInsertQuery(long timestamp, float value, bool isDayDosage)
        {
            if (CultureInfo.CurrentCulture != Program.CULTURE)
            {
                Program.SetCulture(Program.CULTURE);
            }
            string INSERT_QUERY = "INSERT INTO `insulin` (`timestamp`,`value`,`dayDosage`) VALUES (FROM_UNIXTIME(%d * 0.001),%f,%d)";
            return string.Format(INSERT_QUERY, timestamp, value, getBooleanAsInteger(isDayDosage));
        }

        public static string GetInsertQuery(List<Insulin> items)
        {
            if (CultureInfo.CurrentCulture != Program.CULTURE)
            {
                Program.SetCulture(Program.CULTURE);
            }
            var insertQuery = "INSERT INTO `insulin` (`timestamp`,`value`,`dayDosage`) VALUES ";

            foreach (Insulin item in items)
            {
                insertQuery += $"(FROM_UNIXTIME({item.getTimestamp()} * 0.001),{item.getValue():N1},{getBooleanAsInteger(item.isIsDayDosage())}),";
            }

            return insertQuery.Substring(0, insertQuery.Length - 1);
        }

        private static int getBooleanAsInteger(bool b)
        {
            return b ? 1 : 0;
        }
    }
}
