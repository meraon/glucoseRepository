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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GlukAppWpf.Models;
using GlukAppWpf.ViewModels;

namespace GlukAppWpf
{
    public class ModelProvider : ObservableObject
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

        public void AddGlucoseDatapoint(DataPoint point)
        {
            var item = GlucoseDataPoints.FirstOrDefault(p => p.X > point.X);
            if (item.Equals(new DataPoint()))
            {
                GlucoseDataPoints.Add(point);
            }
            else
            {
                var index = GlucoseDataPoints.IndexOf(item);
                GlucoseDataPoints.Insert(index, point);
            }
        }

        public void AddInsulinDatapoint(DataPoint point)
        {
            var item = InsulinDataPoints.FirstOrDefault(p => p.X > point.X);
            if (item.Equals(new DataPoint()))
            {
                InsulinDataPoints.Add(point);
            }
            else
            {
                var index = InsulinDataPoints.IndexOf(item);
                InsulinDataPoints.Insert(index, point);
            }
        }

        private void LoadData()
        {
            LoadDataFromDb();
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

        private void PopulateGlucoseDataPoints()
        {
            foreach (var glucose in Glucoses)
            {
                GlucoseDataPoints.Add(glucose.GetDataPoint());
            };

            GlucoseDataPoints.Sort(point => point.X);
        }

        private void PopulateInsulinDataPoints()
        {
            foreach (var insulin in Insulins)
            {
                InsulinDataPoints.Add(insulin.GetDataPoint());
            }

            InsulinDataPoints.Sort(point => point.X);
        }

        public static DataPoint ModelToDataPoint(ItemBase item)
        {
            return new DataPoint(DateTimeAxis.ToDouble(item.Date), item.Value);
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

                        insulin.PropertyChanged += InsulinOnPropertyChanged;

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
                        glucose.GenerateDataPoint();
                        glucose.PropertyChanged += GlucoseOnPropertyChanged;

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

        private void GlucoseOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var glucose = (GlucoseItem) sender;
            var oldPoint = glucose.GetDataPoint();
            GlucoseDataPoints.Remove(oldPoint);
            AddGlucoseDatapoint(glucose.GenerateDataPoint());
            RaisePropertyChanged(() => GlucoseDataPoints);
        }

        private void InsulinOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var insulin = (InsulinItem)sender;
            var oldPoint = insulin.GetDataPoint();
            InsulinDataPoints.Remove(oldPoint);
            AddInsulinDatapoint(insulin.GenerateDataPoint());
            RaisePropertyChanged(() => InsulinDataPoints);
        }

        private void GlucoseCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    foreach (GlucoseItem glucose in args.NewItems)
                    {
                        AddGlucoseDatapoint(glucose.GetDataPoint());
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (GlucoseItem glucose in args.OldItems)
                    {
                        GlucoseDataPoints.Remove(glucose.GetDataPoint());
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    int count = args.OldItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var glucose = args.OldItems[i] as GlucoseItem;
                        if (!GlucoseDataPoints.Contains(glucose.GetDataPoint()))
                        {
                            throw new ValueUnavailableException("Datapoint is missing.");
                        }

                        GlucoseDataPoints.Remove(glucose.GetDataPoint());
                        if (args.NewItems[i] is GlucoseItem newGlucose) AddGlucoseDatapoint(newGlucose.GetDataPoint());
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
                        AddInsulinDatapoint(insulin.GetDataPoint());
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
                        if (args.NewItems[i] is InsulinItem newInsulin) AddInsulinDatapoint(newInsulin.GetDataPoint());
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

        


    }
    
}
