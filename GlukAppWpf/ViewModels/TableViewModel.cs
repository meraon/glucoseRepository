using System;
using GalaSoft.MvvmLight;
using OxyPlot;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace GlukAppWpf.ViewModels
{
    public class TableViewModel : ViewModelBase
    {
        private ModelProvider _modelController;
        private DataGrid _dataGrid;

        public DataSource Source;

        public TableViewModel()
        {
            

        }

        public TableViewModel(DataGrid dataGrid, ModelProvider modelController) : this()
        {
            _modelController = modelController;
            _dataGrid = dataGrid;
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
    }
}
