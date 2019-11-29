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
                var highlights = GetCurrentHighlightCollection();
                foreach (var addedItem in args.AddedItems)
                {
                    highlights.Add(((ItemBase)addedItem).GetScatterPoint());
                }
                foreach (var removedItem in args.RemovedItems)
                {
                    var point = ((ItemBase) removedItem).GetScatterPoint();

                    highlights.Remove(point);
                }
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
            var dataSourcePropertyName = nameof(source.Source);
            source.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(dataSourcePropertyName))
                {
                    SetItemsSource(source.Source);  
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
            switch (Source.Source)
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
