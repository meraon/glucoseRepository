using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GlukAppWpf.Pages;
using GlukLibrary;
using GlukLibrary.DbQuery;
using MySql.Data.MySqlClient;
using OxyPlot;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace GlukAppWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand ExportCommand { get; }
        public RelayCommand ImportCommand { get; }
        public RelayCommand ExitCommand { get; }

        private readonly string _connectionString;
            
        private ObservableCollection<Glucose> _glucoses = new ObservableCollection<Glucose>();
        private ObservableCollection<Insulin> _insulins = new ObservableCollection<Insulin>();

        private Frame _mainFrame;


        public ObservableCollection<DataPoint> Points { get; set; }

        public MainViewModel()
        {
            _glucoses.CollectionChanged += (sender, args) => RaisePropertyChanged(() => _glucoses);
            _insulins.CollectionChanged += (sender, args) => RaisePropertyChanged(() => _insulins);

            ExitCommand = new RelayCommand(Exit);
 
        }

        public MainViewModel(Frame mainFrame) : this()
        {
            _mainFrame = mainFrame;
            var modelController = new ModelController();
            modelController.LoadData();
            _mainFrame.Content = new GraphPage(modelController);
        }

        public MainViewModel(Frame mainFrame, Frame secondFrame) : this(mainFrame)
        {
            secondFrame.Content = new TablePage();

            mainFrame.GotFocus += (sender, args) =>
            {
                if (sender is GraphPage page)
                {
                    page.Refresh();
                }
            };
        }

        private void GetDataFromDb()
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
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



        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
