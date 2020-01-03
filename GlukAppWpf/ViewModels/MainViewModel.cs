using System;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GlukAppWpf.Pages;
using System.Windows;
using System.Windows.Controls;
using GlukAppWpf.Models;
using Microsoft.Win32;
using NLog;

namespace GlukAppWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Logger LOG = LogManager.GetCurrentClassLogger();

        public RelayCommand GraphCommand { get; private set; }
        public RelayCommand TableCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }
        public RelayCommand ExportCommand { get; private set; }
        public RelayCommand ImportCommand { get; private set; }
        public RelayCommand GlucosesSourceCommand { get; private set; }
        public RelayCommand InsulinsSourceCommand { get; private set; }


        private Frame _graphFrame;
        private Frame _tableFrame;
        private ModelProvider _modelController;
        private DataSource _dataSource;

        public MainViewModel()
        {
            InitCommands();
            _dataSource = new DataSource();
        }

        public MainViewModel(Frame graphFrame) : this()
        {
            _graphFrame = graphFrame;
            _modelController = new ModelProvider();
            _graphFrame.Content = new GraphPage(_modelController, _dataSource);
        }

        public MainViewModel(Frame graphFrame, Frame tableFrame) : this(graphFrame)
        {
            _tableFrame = tableFrame;
            _tableFrame.Content = new TablePage(_modelController, _dataSource);
        }

        private void InitCommands()
        {
            GraphCommand = new RelayCommand(ShowGraph);
            TableCommand = new RelayCommand(ShowTable);
            ExitCommand = new RelayCommand(Exit);
            ExportCommand = new RelayCommand(Export);
            ImportCommand = new RelayCommand(Import);
            GlucosesSourceCommand = new RelayCommand(SetSourceGlucoses);
            InsulinsSourceCommand = new RelayCommand(SetSourceInsulins);
        }

        private void HighlightPoint(ItemBase item)
        {
            var point = _modelController.GlucoseDataPoints.FirstOrDefault(x => x.Equals(item.GetDataPoint()));
            if (!point.IsDefined())
            {
                LOG.Warn("Point is missing!");
                return;
            }

            var model = ((GraphPage) _graphFrame.Content)?.Plot.ActualModel;
            if (model == null)
            {
                LOG.Warn("Unable to get plot model");
                return;
            }



            
        }

        private void ShowGraph()
        {
            _graphFrame.Content = new GraphPage(_modelController, _dataSource);
            
        }

        private void ShowTable()
        {
            _graphFrame.Content = new TablePage(_modelController, _dataSource);
            
        }

        private void SetSourceGlucoses()
        {
            _dataSource.Current = DataSources.Glucoses;
        }

        private void SetSourceInsulins()
        {
            _dataSource.Current = DataSources.Insulins;
        }

        private void Export()
        {
            try
            {
                var workbook = Serializer.Export(_modelController.Glucoses, _modelController.Insulins);

                SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Excel 2007 file (*.xlsx)|*.xlsx" };
                if (saveFileDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Import()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Excel 2007 file (*.xlsx)|*.xlsx" };
                if (openFileDialog.ShowDialog() == true)
                {
                    Serializer.Import(openFileDialog.FileName, _modelController.Glucoses, _modelController.Insulins);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
