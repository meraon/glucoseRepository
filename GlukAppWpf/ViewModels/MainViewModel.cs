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

        private Frame _mainFrame;
        private ModelController _modelController;

        public MainViewModel()
        {
            ExitCommand = new RelayCommand(Exit);
        }

        public MainViewModel(Frame mainFrame) : this()
        {
            _mainFrame = mainFrame;
            _modelController = new ModelController();
            _mainFrame.Content = new GraphPage(_modelController);
        }


        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
