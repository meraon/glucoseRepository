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

        private ModelProvider _modelController;

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

        public GraphViewModel(PlotModel model, ModelProvider modelController) : this()
        {
            Model = model;
            _modelController = modelController;
        }

        public GraphViewModel(PlotModel model, ModelProvider modelController, DataSource dataSource) : this(model, modelController)
        {
            var dataSourcePropertyName = nameof(dataSource.Source);
            dataSource.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(dataSourcePropertyName))
                {
                    ChangeDataSource(dataSource.Source);
                }
            };
        }


        public GraphViewModel(PlotModel model, ObservableCollection<DataPoint> points)
        {
            Model = model;
            Points = points;
        }

        private void ChangeDataSource(DataSources source)
        {
            switch (source)
            {
                case DataSources.Glucoses:
                    Points = _modelController.GlucoseDataPoints;
                    break;
                case DataSources.Insulins:
                    Points = _modelController.InsulinDataPoints;
                    break;
            }
        }
    }
}
