using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace GlukLibrary.DbQuery
{
    public class GlucoseTable
    {
        private const string _tableName = "glucose";
        public static string TableName => "`" + _tableName + "`";

        private const string _id = "_id";
        public static string Id => "`" + _id + "`";
        private const string _value = "value";
        public static string Value => "`" + _value + "`";
        private const string _timestamp = "timestamp";
        public static string Timestamp => "`" + _timestamp + "`";

        public static readonly string CreateTable = "CREATE TABLE IF NOT EXISTS " + TableName +
                                                    " (" + Id + " INT NOT NULL AUTO_INCREMENT," +
                                                    Value + " FLOAT NOT NULL DEFAULT '0'," +
                                                    Timestamp + " TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP," +
                                                    "PRIMARY KEY (" + Id + "))";

        
        public static readonly string SelectAll = "SELECT * FROM " + TableName;
        public static readonly string DeleteQuery = "DELETE FROM " + TableName + " WHERE ";
        public static readonly string UpdateQuery = "UPDATE " + TableName + " SET "
            + Value + "={0},"
            + Timestamp + "={1} "
            + "WHERE "
            + Id + "={2}";

        //INSERT
        public static string GetInsertQuery(long timestamp, float value)
        {
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
        //DELETE
        public static string GetDeleteQuery(Glucose item)
        {
            string query = DeleteQuery + Id + "=" + item.getId();
            return query;
        }
        public static string GetDeleteQuery(List<Glucose> items)
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
        //UPDATE
        public static string GetUpdateQuery(Glucose item)
        {
            string query = String.Format(UpdateQuery, item.getValue(), "FROM_UNIXTIME(" + item.getTimestamp() + "*0.001)", item.getId());
            return query;
        }

    }
}
