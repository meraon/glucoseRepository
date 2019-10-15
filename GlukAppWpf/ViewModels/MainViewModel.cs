using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GlukAppWpf.Pages;
using OxyPlot;

namespace GlukAppWpf.ViewModels
{
    public class MainViewModel
    {
        public RelayCommand ExportCommand { get; private set; }
        public RelayCommand ImportCommand { get; private set; }

        private Frame _mainFrame;

        public ObservableCollection<DataPoint> Points { get; set; }

        public MainViewModel()
        {
            
        }

        public MainViewModel(Frame mainFrame) : this()
        {
            _mainFrame = mainFrame;
            mainFrame.Content = new GraphPage();
        }
    }
}
