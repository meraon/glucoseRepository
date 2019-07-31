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
        public static readonly string TABLE_NAME = "insulin";
        public static readonly string CREATE_TABLE = "CREATE TABLE `insulin` (`_id` INT NOT NULL AUTO_INCREMENT,`timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,`value` FLOAT NOT NULL DEFAULT '0',`dayDosage` TINYINT NOT NULL DEFAULT '1',PRIMARY KEY (`_id`));";

        public InsulinTable()
        {
        }

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
            var INSERT_QUERY = "INSERT INTO `insulin` (`timestamp`,`value`,`dayDosage`) VALUES ";

            Insulin item;
            for (Iterator var2 = items.iterator(); var2.hasNext(); INSERT_QUERY = INSERT_QUERY + System.String.format(, "(FROM_UNIXTIME(%d * 0.001),%f,%d),", item.getTimestamp(), item.getValue(), getBooleanAsInteger(item.isIsDayDosage())))
            {
                item = (Insulin)var2.next();
            }

            return INSERT_QUERY.Substring(0, INSERT_QUERY.Length - 1);
        }

        private static int getBooleanAsInteger(bool b)
        {
            return b ? 1 : 0;
        }
    }
}
