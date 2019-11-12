using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlukLibrary;
using GlukLibrary.DbQuery;
using MySql.Data.MySqlClient;
using NLog;
using OxyPlot;
using OxyPlot.Axes;

namespace GlukAppWpf
{
    public class ModelController
    {
        private static readonly Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private string _connectionString;

        public Dictionary<DataPoint, Glucose> GlucoseDatapoints { get;  set; }
        public Dictionary<DataPoint, Insulin> InsulinDatapoints { get;  set; }

        public ObservableCollection<DataPoint>  GlucosePoints { get; private set; }
        public ObservableCollection<DataPoint> InsulinPoints { get; private set; }

        public ModelController()
        {
            _connectionString = Connection.GetConnectionString(Directory.GetCurrentDirectory());
            GlucoseDatapoints = new Dictionary<DataPoint, Glucose>();
            InsulinDatapoints = new Dictionary<DataPoint, Insulin>();
            
        }
        public ModelController(string connectionString) : this()
        {
            _connectionString = connectionString;
        }

        private void GlucoseCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var point in args.NewItems)
                    {
                        DataPoint dataPoint = (DataPoint)point;
                        GlucoseDatapoints.Add(dataPoint, GetGlucoseFromDatapoint(dataPoint));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var point in args.OldItems)
                    {
                        DataPoint dataPoint = (DataPoint)point;
                        GlucoseDatapoints.Remove(dataPoint);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < args.OldItems.Count; i++)
                    {
                        DataPoint oldPoint = (DataPoint)args.OldItems[i];
                        DataPoint newPoint = (DataPoint)args.NewItems[i];
                        Glucose glucose = GlucoseDatapoints[oldPoint];
                        GlucoseDatapoints.Remove(oldPoint);
                        UpdateGlucose(glucose, newPoint);
                        GlucoseDatapoints.Add(newPoint, glucose);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    GlucoseDatapoints.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //TODO notify collection changed
        }

        private void UpdateGlucose(Glucose glucose, DataPoint point)
        {
            glucose.setTimestamp(HelperMethods.DateTimeToTimestamp(DateTimeAxis.ToDateTime(point.X)));
            glucose.setValue((float) point.Y);
        }

        private Glucose GetGlucoseFromDatapoint(DataPoint dataPoint)
        {
            return new Glucose(HelperMethods.DateTimeToTimestamp(DateTimeAxis.ToDateTime(dataPoint.X)),
                (float)dataPoint.Y);
        }

        private void InsulinCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
           
        }
        

        public void LoadData()
        {
            LoadDataFromDb();
            FillDataPointCollections();
        }


        private void LoadDataFromDb()
        {
            
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                //Glucoses
                GlucoseDatapoints.Clear();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GlucoseTable.SelectAll;
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Glucose glucose = new Glucose(reader.GetInt32("_id"),
                                HelperMethods.DateTimeStringToTimestamp(reader.GetString("timestamp")),
                            reader.GetFloat("value"));
                            DataPoint dataPoint = ModelToDataPoint(glucose);

                            try
                            {
                                GlucoseDatapoints.Add(dataPoint, glucose);
                            }
                            catch (ArgumentException e)
                            {
                                LOG.Warn(e.ToString());
                            }
                        }
                    }
                }

                //Insulins
                InsulinDatapoints.Clear();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = InsulinTable.SelectAll;
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Insulin insulin = new Insulin(reader.GetInt32("_id"),
                                HelperMethods.DateTimeStringToTimestamp(reader.GetString("timestamp")),
                                reader.GetFloat("value"),
                                reader.GetInt16("dayDosage") == 1
                            );
                            DataPoint dataPoint = ModelToDataPoint(insulin);

                            try
                            {
                                InsulinDatapoints.Add(dataPoint, insulin);
                            }
                            catch (ArgumentException e)
                            {
                                LOG.Warn(e.ToString());
                            }
                            
                        }
                    }
                }

            }
        }

        private void FillDataPointCollections()
        {
            GlucosePoints = new ObservableCollection<DataPoint>(GlucoseDatapoints.Keys);
            InsulinPoints = new ObservableCollection<DataPoint>(InsulinDatapoints.Keys);
            GlucosePoints.CollectionChanged += GlucoseCollectionChanged;

        }

        private DataPoint ModelToDataPoint(EntryModel item)
        {
            return new DataPoint(DateTimeAxis.ToDouble(HelperMethods.TimestampToDateTime(item.getTimestamp())), item.getValue());
        }
    }
        

        


    
}
