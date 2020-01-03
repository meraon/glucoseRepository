using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;


//TODO reset graph when changing views

namespace GlukAppWpf.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
        private readonly double _maxValue = 10.0;
        private readonly double _minValue = 5.0;

        private ModelProvider _modelController;


        public PlotModel Model;

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

        private ObservableCollection<DataPoint> _minLine;
        public ObservableCollection<DataPoint> MinLine
        {
            get => _minLine;
            set
            {
                _minLine = value;
                RaisePropertyChanged(() => MinLine);
            }
        }

        private ObservableCollection<DataPoint> _maxLine;
        public ObservableCollection<DataPoint> MaxLine
        {
            get => _maxLine;
            set
            {
                _maxLine = value;
                RaisePropertyChanged(() => MaxLine);
            }
        }

        public GraphViewModel()
        {
            Points = new ObservableCollection<DataPoint>();
            MinLine = new ObservableCollection<DataPoint>();
            MaxLine = new ObservableCollection<DataPoint>();
        }

        public GraphViewModel(PlotModel model) : this()
        {
            Model = model;
        }

        public GraphViewModel(PlotModel model, ObservableCollection<DataPoint> points) : this()
        {
            Model = model;
            Points = points;
            UpdateLine(MinLine, _minValue);
            UpdateLine(MaxLine, _maxValue);
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
            var dataSourcePropertyName = nameof(dataSource.Current);
            dataSource.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(dataSourcePropertyName))
                {
                    ChangeDataSource(dataSource.Current);
                }
            };

            ChangeDataSource(dataSource.Current);
            UpdateLine(MinLine, _minValue);
            UpdateLine(MaxLine, _maxValue);
        }

        private void DataPointsOnCollectionChanged(object o, NotifyCollectionChangedEventArgs args)
        {
            UpdateLine(MinLine, _minValue);
            UpdateLine(MaxLine, _maxValue);
            Application.Current.Dispatcher?.Invoke(() =>
            {
                Model.InvalidatePlot(true);
                Model.ResetAllAxes();
            });
        }

        private void UpdateLine(ObservableCollection<DataPoint> collection, double value)
        {
            collection.Clear();

            var min = 0.0;
            var max = 0.0;
            if (Points.Count > 0)
            {
                min = Points.Min(point => point.X);
                max = Points.Max(point => point.X);
            }
            
            collection.Add(new DataPoint(min, value));
            collection.Add(new DataPoint(max, value));
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
