using GalaSoft.MvvmLight;
using OxyPlot;
using System.Collections.ObjectModel;

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
