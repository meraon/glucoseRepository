using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GlukModels.DbQuery
{
    public class InsulinTable
    {
        private static readonly string _tableName = "insulin";
        public static string TableName => "`" + _tableName + "`";
        private const string _id = "_id";
        public static string Id => "`" + _id + "`";
        private const string _value = "value";
        public static string Value => "`" + _value + "`";
        private const string _timestamp = "timestamp";
        public static string Timestamp => "`" + _timestamp + "`";
        private const string _dayDosage = "dayDosage";
        public static string DayDosage => "`" + _dayDosage + "`";
        public static readonly string CreateTable = "CREATE TABLE IF NOT EXISTS " + TableName +
                                                    " (" + Id + " INT NOT NULL AUTO_INCREMENT," +
                                                    Value + " FLOAT NOT NULL DEFAULT '0'," +
                                                    DayDosage + " TINYINT(1) NOT NULL DEFAULT '1'," +
                                                    Timestamp + " TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                                                    "PRIMARY KEY (" + Id + "))";
        public static readonly string SelectAll = "SELECT * FROM " + TableName + "ORDER BY " + Timestamp + " ASC";
        public static readonly string DeleteQuery = "DELETE FROM " + TableName + " WHERE ";
        public static readonly string UpdateQuery = "DELETE FROM " + TableName + " WHERE ";

        //INSERT
        public static string GetInsertQuery(long timestamp, float value, bool isDayDosage)
        {
            string INSERT_QUERY = "INSERT INTO " + TableName + " (" + Timestamp + "," + Value + "," + DayDosage + ") VALUES (FROM_UNIXTIME(%d * 0.001),%f,%d)";
            return string.Format(INSERT_QUERY, timestamp, value, getBooleanAsInteger(isDayDosage));
        }
        public static string GetInsertQuery(List<Insulin> items)
        {
            var insertQuery = "INSERT INTO " + TableName + " (" + Timestamp + "," + Value + "," + DayDosage + ") VALUES ";

            foreach (Insulin item in items)
            {
                insertQuery += $"(FROM_UNIXTIME({item.getTimestamp()} * 0.001),{item.getValue():N1},{getBooleanAsInteger(item.isIsDayDosage())}),";
            }
            return insertQuery.Substring(0, insertQuery.Length - 1);
        }
        //DELETE
        public static string GetDeleteQuery(Insulin item)
        {
            string query = DeleteQuery + Id + "=" + item.getId();
            return query;
        }
        public static string GetDeleteQuery(List<Insulin> items)
        {
            if (!(items?.Count > 1))
            {
                throw new ArgumentException("List is null or empty");
            }
            string query = DeleteQuery;
            foreach (var glucose in items)
            {
                query += Id + "=" + glucose.getId() + " OR ";
            }
            return query.Substring(0, query.Length - 4);
        }

        private static int getBooleanAsInteger(bool b)
        {
            return b ? 1 : 0;
        }
    }
}
