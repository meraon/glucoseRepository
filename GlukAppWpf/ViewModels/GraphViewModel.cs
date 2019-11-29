using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GlukLibrary;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using OxyPlot.Series;

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

        private ObservableCollection<ScatterPoint> _highlights;
        public ObservableCollection<ScatterPoint> Highlights
        {
            get => _highlights;
            set
            {
                _highlights = value;
                RaisePropertyChanged(() => Highlights);
            }
        }

        public GraphViewModel()
        {
            Points = new ObservableCollection<DataPoint>();
        }

        public GraphViewModel(PlotModel model) : this()
        {
            Model = model;
        }

        public GraphViewModel(PlotModel model, ModelProvider modelController) : this()
        {
            Model = model;
            _modelController = modelController;
            _modelController.GlucoseDataPoints.CollectionChanged += DataPointsOnCollectionChanged;
            _modelController.InsulinDataPoints.CollectionChanged += DataPointsOnCollectionChanged;

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

            ChangeDataSource(dataSource.Source);
        }

        private void DataPointsOnCollectionChanged(object o, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                Model.InvalidatePlot(true);
                Model.ResetAllAxes();
            });
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
                    Highlights = _modelController.GlucoseHighlights;
                    break;
                case DataSources.Insulins:
                    Points = _modelController.InsulinDataPoints;
                    Highlights = _modelController.InsulinHighlights;
                    break;
            }
        }
    }
}
