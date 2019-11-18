using GlukLibrary;
using GlukLibrary.DbQuery;
using MySql.Data.MySqlClient;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace GlukAppWpf
{
    public class ModelController
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _connectionString;

        public ObservableCollection<Glucose> Glucoses { get; private set; }
        public ObservableCollection<Insulin> Insulins { get; private set; }

        public ModelController()
        {
            _connectionString = Connection.GetConnectionString(Directory.GetCurrentDirectory());
            LoadData();
        }

        public ModelController(string connectionString) : this()
        {
            _connectionString = connectionString;
            LoadData();
        }

        private void LoadData()
        {
            LoadDataFromDb();
        }

        public ObservableCollection<DataPoint> GetGlucoseDataPoints()
        {
            ObservableCollection<DataPoint> points = new ObservableCollection<DataPoint>();
            foreach (var glucose in Glucoses)
            {
                points.Add(ModelToDataPoint(glucose));
            }

            return points;
        }

        public ObservableCollection<DataPoint> GetInsulinDataPoints()
        {
            ObservableCollection<DataPoint> points = new ObservableCollection<DataPoint>();
            foreach (var insulin in Insulins)
            {
                points.Add(ModelToDataPoint(insulin));
            }

            return points;
        }

        private void LoadDataFromDb()
        {

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                Glucoses = GetGlucosesFromDb(conn);
                Insulins = GetInsulinsFromDb(conn);
            }
        }

        private ObservableCollection<Insulin> GetInsulinsFromDb(MySqlConnection conn)
        {
            try
            {
                ObservableCollection<Insulin> insulins = new ObservableCollection<Insulin>();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = InsulinTable.SelectAll;
                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows) return null;

                    while (reader.Read())
                    {
                        Insulin insulin = new Insulin(reader.GetInt32("_id"),
                            HelperMethods.DateTimeStringToTimestamp(reader.GetString("timestamp")),
                            reader.GetFloat("value"),
                            reader.GetInt16("dayDosage") == 1
                        );
                        try
                        {
                            insulins.Add(insulin);
                        }
                        catch (ArgumentException e)
                        {
                            LOG.Warn(e.ToString());
                        }
                    }
                }

                return insulins;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private ObservableCollection<Glucose> GetGlucosesFromDb(MySqlConnection conn)
        {
            try
            {
                ObservableCollection<Glucose> glucoses = new ObservableCollection<Glucose>();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GlucoseTable.SelectAll;
                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows) return null;

                    while (reader.Read())
                    {
                        Glucose glucose = new Glucose(reader.GetInt32("_id"),
                            HelperMethods.DateTimeStringToTimestamp(reader.GetString("timestamp")),
                            reader.GetFloat("value"));
                        try
                        {
                            glucoses.Add(glucose);
                        }
                        catch (ArgumentException e)
                        {
                            LOG.Warn(e.ToString());
                        }
                    }
                }

                return glucoses;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private DataPoint ModelToDataPoint(EntryModel item)
        {
            return new DataPoint(DateTimeAxis.ToDouble(HelperMethods.TimestampToDateTime(item.getTimestamp())), item.getValue());
        }

    }






}
