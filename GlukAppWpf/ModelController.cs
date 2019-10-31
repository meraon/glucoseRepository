using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlukModels;
using GlukModels.DbQuery;
using MySql.Data.MySqlClient;
using OxyPlot;
using OxyPlot.Axes;

namespace GlukAppWpf
{
    public class ModelController
    {
        private string _connectionString;

        private Dictionary<DataPoint, Glucose> GlucoseDatapoints { get;  set; }
        private Dictionary<DataPoint, Insulin> InsulinDatapoints { get;  set; }

        public ObservableCollection<DataPoint>  GlucosePoints { get; private set; }
        public ObservableCollection<DataPoint> InsulinPoints { get; private set; }


        public ModelController()
        {
            _connectionString = Connection.GetConnectionString(Directory.GetCurrentDirectory());
            GlucoseDatapoints = new Dictionary<DataPoint, Glucose>();
            InsulinDatapoints = new Dictionary<DataPoint, Insulin>();
                        
        }

        private void GlucoseCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    List<DataPoint> points = (List<DataPoint>) args.NewItems;
                    foreach (var dataPoint in points)
                    {
                        GlucoseDatapoints.Add(dataPoint, new Glucose(HelperMethods.DateTimeToTimestamp(DateTimeAxis.ToDateTime(dataPoint.X)),
                            (float)dataPoint.Y));
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    List<DataPoint> pointsRemove = (List<DataPoint>) args.NewItems;
                    foreach (var dataPoint in pointsRemove)
                    {
                        GlucosePoints.Remove(dataPoint);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    List<DataPoint> newPoints = (List<DataPoint>)args.NewItems;
                    List<DataPoint> oldPoints = (List<DataPoint>)args.OldItems;

                    for (int i = 0; i < oldPoints.Count; i++)
                    {
                        GlucoseDatapoints.Remove(oldPoints[i]);
                        GlucoseDatapoints.Add(newPoints[i], new Glucose(HelperMethods.DateTimeToTimestamp(DateTimeAxis.ToDateTime(newPoints[i].X)), (float)newPoints[i].Y));
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //TODO notify collection changed
        }

        private void InsulintCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            
        }

        public ModelController(string connectionString) : this()
        {
            _connectionString = connectionString;
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
                            DataPoint dataPoint = GetDataPoint(glucose);
                            GlucoseDatapoints.Add(dataPoint, glucose);
                        }
                    }
                }

                //Insulins
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
                            DataPoint dataPoint = GetDataPoint(insulin);
                            InsulinDatapoints.Add(dataPoint, insulin);
                        }
                    }
                }

            }
        }

        private void FillDataPointCollections()
        {
            GlucosePoints = new ObservableCollection<DataPoint>(GlucoseDatapoints.Keys);
            InsulinPoints = new ObservableCollection<DataPoint>(InsulinDatapoints.Keys);
        }

        private DataPoint GetDataPoint(EntryModel item)
        {
            return new DataPoint(DateTimeAxis.ToDouble(HelperMethods.TimestampToDateTime(item.getTimestamp())), item.getValue());
        }

        


    }
        

        


    
}
