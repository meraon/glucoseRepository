using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using OxyPlot;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using GlukAppWpf.Models;
using OxyPlot.Series;

namespace GlukAppWpf.ViewModels
{
    public class TableViewModel : ViewModelBase
    {
        private ModelProvider _modelController;

        private DataGrid _dataGrid;

        public DataSource Source;

        public TableViewModel(DataGrid dataGrid)
        {
            _dataGrid = dataGrid;
            _dataGrid.SelectionChanged += (sender, args) =>
            {
                foreach (var addedItem in args.AddedItems)
                {
                    var addedPoint = ((ItemBase)addedItem).GetScatterPoint();
                    _modelController.AddScatterPoint(addedPoint, Source.Current);
                }
                foreach (var removedItem in args.RemovedItems)
                {
                    var point = ((ItemBase)removedItem).GetScatterPoint();
                    _modelController.RemoveScatterPoint(point, Source.Current);
                }
            };

            _dataGrid.TargetUpdated += (sender, args) =>
            {
               // args.
            };
        }

        public TableViewModel(DataGrid dataGrid, ModelProvider modelController) : this(dataGrid)
        {
            _modelController = modelController;
            dataGrid.ItemsSource = modelController.Glucoses;

        }

        public TableViewModel(DataGrid dataGrid, ModelProvider modelController, DataSource source) : this(dataGrid, modelController)
        {
            Source = source;
            var dataSourcePropertyName = nameof(source.Current);
            source.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(dataSourcePropertyName))
                {
                    SetItemsSource(source.Current);  
                }
            };
        }

        private void SetItemsSource(DataSources source)
        {
            switch (source)
            {
                case DataSources.Glucoses:
                    _dataGrid.ItemsSource = _modelController.Glucoses;
                    break;
                case DataSources.Insulins:
                    _dataGrid.ItemsSource = _modelController.Insulins;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }

            //_dataGrid.Items.Refresh();
        }

        private ObservableCollection<ScatterPoint> GetCurrentHighlightCollection()
        {
            switch (Source.Current)
            {
                case DataSources.Glucoses:
                    return _modelController.GlucoseHighlights;
                case DataSources.Insulins:
                    return _modelController.InsulinHighlights;
                default: return null;
            }
        }
    }
}
