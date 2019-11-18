using GalaSoft.MvvmLight;
using OxyPlot;
using System.Collections.ObjectModel;

namespace GlukAppWpf.ViewModels
{
    public class TableViewModel : ViewModelBase
    {
        private ModelController _modelController;


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

        public TableViewModel(ModelController modelController) : this()
        {
            _modelController = modelController;
        }
    }
}
