using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GlukLibrary;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace GlukAppWpf.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
        public PlotModel Model;

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

        public GraphViewModel()
        {
            Points = new ObservableCollection<DataPoint>();
            Points.CollectionChanged += (sender, args) =>
            {
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    Model.InvalidatePlot(true);
                });
            };
            

        }

        public GraphViewModel(PlotModel model) : this()
        {
            Model = model;
        }

        public GraphViewModel(PlotModel model, ModelController modelController) : this()
        {
            Model = model;
            _modelController = modelController;
            Points = _modelController.GetGlucoseDataPoints();
            Points.Sort(point => point.X);
        }

        public GraphViewModel(PlotModel model, ObservableCollection<DataPoint> points)
        {
            Model = model;
            Points = points;
        }

        public void Refresh()
        {
            Points.Sort(point => point.X);
        }
    }
}
