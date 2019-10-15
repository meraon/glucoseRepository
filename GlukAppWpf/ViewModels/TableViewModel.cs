using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OxyPlot;

namespace GlukAppWpf.ViewModels
{
    public class TableViewModel : ViewModelBase
    {
        private ObservableCollection<DataPoint> _points;
        public ObservableCollection<DataPoint> Points
        {
            get => _points;
            set
            {
                _points = value;
                RaisePropertyChanged(() => Points);
            }
        }


        public TableViewModel()
        {
            
        }

        public TableViewModel(ObservableCollection<DataPoint> points) : this()
        {
            Points = points;
        }
    }
}
