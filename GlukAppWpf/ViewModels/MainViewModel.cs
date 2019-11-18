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

        private Frame _mainFrame;
        private ModelController _modelController;

        public MainViewModel()
        {
            InitCommands();
        }

        public MainViewModel(Frame mainFrame) : this()
        {
            _mainFrame = mainFrame;
            _modelController = new ModelController();
            _mainFrame.Content = new GraphPage(_modelController);
        }

        private void InitCommands()
        {
            GraphCommand = new RelayCommand(ShowGraph);
            TableCommand = new RelayCommand(ShowTable);
            ExitCommand = new RelayCommand(Exit);
        }

        private void ShowGraph()
        {
            _mainFrame.Content = new GraphPage(_modelController);
        }

        private void ShowTable()
        {
            _mainFrame.Content = new TablePage(_modelController);
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
