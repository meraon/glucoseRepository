using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GlukAppWpf.Pages;
using System.Windows;
using System.Windows.Controls;

namespace GlukAppWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand GraphCommand { get; private set; }
        public RelayCommand TableCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }
        public RelayCommand ExportCommand { get; private set; }
        public RelayCommand ImportCommand { get; private set; }


        private Frame _mainFrame;
        private ModelProvider _modelController;
        private DataSource _dataSource;

        public MainViewModel()
        {
            InitCommands();
            _dataSource = new DataSource();
        }

        public MainViewModel(Frame mainFrame) : this()
        {
            _mainFrame = mainFrame;
            _modelController = new ModelProvider();
            _mainFrame.Content = new GraphPage(_modelController, _dataSource);
        }

        private void InitCommands()
        {
            GraphCommand = new RelayCommand(ShowGraph);
            TableCommand = new RelayCommand(ShowTable);
            ExitCommand = new RelayCommand(Exit);
            ExportCommand = new RelayCommand(Export);
            ImportCommand = new RelayCommand(Import);
        }

        private void ShowGraph()
        {
            _mainFrame.Content = new GraphPage(_modelController, _dataSource);
            
        }

        private void ShowTable()
        {
            _mainFrame.Content = new TablePage(_modelController, _dataSource);
            
        }

        private void Export()
        {
            _dataSource.Source = DataSources.Glucoses;
        }

        private void Import()
        {
            _dataSource.Source = DataSources.Insulins;
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
