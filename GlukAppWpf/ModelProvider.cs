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
using GalaSoft.MvvmLight;
using GlukAppWpf.Models;
using GlukAppWpf.ViewModels;

namespace GlukAppWpf
{
    public class ModelProvider
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _connectionString;

        public ObservableCollection<GlucoseItem> Glucoses { get; private set; }
        public ObservableCollection<InsulinItem> Insulins { get; private set; }

        public ObservableCollection<DataPoint> GlucoseDataPoints { get; private set; }
        public ObservableCollection<DataPoint> InsulinDataPoints { get; private set; }

        public ModelProvider()
        {
            _connectionString = Connection.GetConnectionString(Directory.GetCurrentDirectory());
            LoadData();
        }

        public ModelProvider(string connectionString)
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

        private ObservableCollection<InsulinItem> GetInsulinsFromDb(MySqlConnection conn)
        {
            try
            {
                ObservableCollection<InsulinItem> insulins = new ObservableCollection<InsulinItem>();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = InsulinTable.SelectAll;
                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows) return null;

                    while (reader.Read())
                    {
                        var id = reader.GetInt32("_id");
                        var dateTime = DateTime.Parse(reader.GetString("timestamp"));
                        var value = reader.GetFloat("value");
                        var dayDosage = reader.GetInt16("dayDosage") == 1;
                        InsulinItem insulin = new InsulinItem(id,
                            dateTime, 
                            value,
                            dayDosage
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

        private ObservableCollection<GlucoseItem> GetGlucosesFromDb(MySqlConnection conn)
        {
            try
            {
                ObservableCollection<GlucoseItem> glucoses = new ObservableCollection<GlucoseItem>();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GlucoseTable.SelectAll;
                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows) return null;

                    while (reader.Read())
                    {
                        var id = reader.GetInt32("_id");
                        var dateTime = DateTime.Parse(reader.GetString("timestamp"));
                        var value = reader.GetFloat("value");
                        GlucoseItem glucose = new GlucoseItem(id,
                            dateTime,
                            value);
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

        public static DataPoint ModelToDataPoint(ItemBase item)
        {
            return new DataPoint(DateTimeAxis.ToDouble(item.Date), item.Value);
        }

        private void GlucoseCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    foreach (GlucoseItem glucose in args.NewItems)
                    {
                        AddGlucoseDatapoint(glucose);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (GlucoseItem glucose in args.OldItems)
                    {
                        var point = ModelToDataPoint(glucose);
                        GlucoseDataPoints.Remove(point);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    int count = args.OldItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var glucose = args.OldItems[i] as GlucoseItem;
                        var point = ModelToDataPoint(glucose);
                        if (!GlucoseDataPoints.Contains(point))
                        {
                            throw new ValueUnavailableException("Datapoint is missing.");
                        }

                        GlucoseDataPoints.Remove(point);
                        var newGlucose = args.NewItems[i] as GlucoseItem;
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

                    foreach (InsulinItem insulin in args.NewItems)
                    {
                        AddInsulinDatapoint(insulin);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (InsulinItem insulin in args.OldItems)
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
                        var insulin = args.OldItems[i] as InsulinItem;
                        var point = ModelToDataPoint(insulin);
                        if (!InsulinDataPoints.Contains(point))
                        {
                            throw new ValueUnavailableException("Datapoint is missing.");
                        }

                        InsulinDataPoints.Remove(point);
                        var newInsulin = args.NewItems[i] as InsulinItem;
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

        public void AddGlucoseDatapoint(GlucoseItem glucose)
        {
            var timestamp =
                DateTimeAxis.ToDouble(glucose.Date);
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

        public void AddInsulinDatapoint(InsulinItem insulin)
        {
            var timestamp =
                DateTimeAxis.ToDouble(insulin.Date);
            var item = InsulinDataPoints.FirstOrDefault(point => point.X > timestamp);
            if (item.Equals(new DataPoint()))
            {
                InsulinDataPoints.Add(ModelToDataPoint(insulin));
            }
            else
            {
                var index = InsulinDataPoints.IndexOf(item);
                InsulinDataPoints.Insert(index, ModelToDataPoint(insulin));
            }
        }
    }
    
}
