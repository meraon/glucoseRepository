using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GlukAppWpf.Pages;
using GlukModels;
using GlukModels.DbQuery;
using MySql.Data.MySqlClient;
using OxyPlot;
using OxyPlot.Axes;

namespace GlukAppWpf.ViewModels
{
    public class MainViewModel
    {
        public RelayCommand ExportCommand { get; private set; }
        public RelayCommand ImportCommand { get; private set; }

        private List<Glucose> _glucoses = new List<Glucose>();
        private List<Insulin> _insulins = new List<Insulin>();

        private Frame _mainFrame;


        public ObservableCollection<DataPoint> Points { get; set; }

        public MainViewModel()
        {
            GetDataFromDb();
        }

        public MainViewModel(Frame mainFrame) : this()
        {
            _mainFrame = mainFrame;
            mainFrame.Content = new GraphPage();
        }

        private void GetDataFromDb()
        {
            string connectionString = Connection.GetConnectionString(Directory.GetCurrentDirectory());

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GlucoseTable.SelectAll;
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _glucoses.Add(new Glucose(reader.GetInt32("_id"),
                                HelperMethods.DateTimeStringToTimestamp(reader.GetString("timestamp")),
                                reader.GetFloat("value")
                            ));
                        }
                    }
                }


                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = InsulinTable.SelectAll;
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _insulins.Add(new Insulin(reader.GetInt32("_id"),
                                HelperMethods.DateTimeStringToTimestamp(reader.GetString("timestamp")),
                                reader.GetFloat("value"),
                                reader.GetInt16("dayDosage") == 1
                            ));
                        }
                    }
                }

            }
        }
    }
}
