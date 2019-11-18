using GlukLibrary;
using GlukLibrary.DbQuery;
using MySql.Data.MySqlClient;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace GlukAppWpf
{
    public class ModelController
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _connectionString;

        public ObservableCollection<Glucose> Glucoses { get; private set; }
        public ObservableCollection<Insulin> Insulins { get; private set; }

        public ObservableCollection<DataPoint> GlucoseDataPoints { get; private set; }
        public ObservableCollection<DataPoint> InsulinDataPoints { get; private set; }

        public ModelController()
        {
            _connectionString = Connection.GetConnectionString(Directory.GetCurrentDirectory());
            LoadData();
        }

        public ModelController(string connectionString)
        {
            _connectionString = connectionString;
            LoadData();
        }

        private void LoadData()
        {
            LoadDataFromDb();
        }

        private void PopulateGlucoseDataPoints()
        {
            foreach (var glucose in Glucoses)
            {
                GlucoseDataPoints.Add(ModelToDataPoint(glucose));
            };

            GlucoseDataPoints.Sort(point => point.X);
        }

        private void PopulateInsulinDataPoints()
        {
            foreach (var insulin in Insulins)
            {
                InsulinDataPoints.Add(ModelToDataPoint(insulin));
            }

            InsulinDataPoints.Sort(point => point.X);
        }

        private void LoadDataFromDb()
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                Glucoses = GetGlucosesFromDb(conn);
                Insulins = GetInsulinsFromDb(conn);
            }

            GlucoseDataPoints = new ObservableCollection<DataPoint>();
            InsulinDataPoints = new ObservableCollection<DataPoint>();

            PopulateGlucoseDataPoints();
            PopulateInsulinDataPoints();

            Glucoses.CollectionChanged += GlucoseCollectionChanged;
            Insulins.CollectionChanged += InsulinCollectionChanged;
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

        public static DataPoint ModelToDataPoint(EntryModel item)
        {
            return new DataPoint(DateTimeAxis.ToDouble(HelperMethods.TimestampToDateTime(item.getTimestamp())), item.getValue());
        }

        private void GlucoseCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    foreach (Glucose glucose in args.NewItems)
                    {
                        AddGlucoseDatapoint(glucose);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (Glucose glucose in args.OldItems)
                    {
                        var point = ModelToDataPoint(glucose);
                        GlucoseDataPoints.Remove(point);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    int count = args.OldItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var glucose = args.OldItems[i] as Glucose;
                        var point = ModelToDataPoint(glucose);
                        if (!GlucoseDataPoints.Contains(point))
                        {
                            throw new ValueUnavailableException("Datapoint is missing.");
                        }

                        GlucoseDataPoints.Remove(point);
                        var newGlucose = args.NewItems[i] as Glucose;
                        AddGlucoseDatapoint(newGlucose);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    GlucoseDataPoints.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InsulinCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    foreach (Insulin insulin in args.NewItems)
                    {
                        AddInsulinDatapoint(insulin);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (Insulin insulin in args.OldItems)
                    {
                        var point = ModelToDataPoint(insulin);
                        if (!InsulinDataPoints.Remove(point))
                        {
                            throw new InvalidOperationException($"Insulin {insulin.ToString()} datapoint {point} was not removed");
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    int count = args.OldItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var insulin = args.OldItems[i] as Insulin;
                        var point = ModelToDataPoint(insulin);
                        if (!InsulinDataPoints.Contains(point))
                        {
                            throw new ValueUnavailableException("Datapoint is missing.");
                        }

                        InsulinDataPoints.Remove(point);
                        var newInsulin = args.NewItems[i] as Insulin;
                        AddInsulinDatapoint(newInsulin);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    InsulinDataPoints.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddGlucoseDatapoint(Glucose glucose)
        {
            var timestamp =
                DateTimeAxis.ToDouble(HelperMethods.TimestampToDateTime(glucose.getTimestamp()));
            var item = GlucoseDataPoints.FirstOrDefault(point => point.X > timestamp);
            if (item.Equals(new DataPoint()))
            {
                GlucoseDataPoints.Add(ModelToDataPoint(glucose));
            }
            else
            {
                var index = GlucoseDataPoints.IndexOf(item);
                GlucoseDataPoints.Insert(index, ModelToDataPoint(glucose));
            }
        }

        public void AddInsulinDatapoint(Insulin insulin)
        {
            var timestamp =
                DateTimeAxis.ToDouble(HelperMethods.TimestampToDateTime(insulin.getTimestamp()));
            var item = InsulinDataPoints.FirstOrDefault(point => point.X > timestamp);
            if (item.Equals(new DataPoint()))
            {
                InsulinDataPoints.Add(ModelToDataPoint(insulin));
            }
            else
            {
                var index = InsulinDataPoints.IndexOf(item);
                GlucoseDataPoints.Insert(index, ModelToDataPoint(insulin));
            }
        }
    }






}
