using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GlukAppWpf.Annotations;
using GlukModels;
using OxyPlot;
using OxyPlot.Axes;
using PlotCommands = OxyPlot.Wpf.PlotCommands;
using Timer = System.Timers.Timer;

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

        public RelayCommand LoadDataCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand SortCommand { get; private set; }

        private string _loadText;
        public string LoadText {
            get => _loadText;
            set
            {
                _loadText = value;
                RaisePropertyChanged(() => LoadText);
            } }

        private string _addText;
        public string AddText
        {
            get => _addText;
            set
            {
                _addText = value;
                RaisePropertyChanged(() => AddText);
            }
        }

        private string _removeText;
        public string RemoveText
        {
            get => _removeText;
            set
            {
                _removeText = value;
                RaisePropertyChanged(() => RemoveText);
            }
        }

        private string _updateText;
        public string UpdateText
        {
            get => _updateText;
            set
            {
                _updateText = value;
                RaisePropertyChanged(() => UpdateText);
            }
        }

        private void SetCommands()
        {
            LoadDataCommand = new RelayCommand(() =>
            {
                _modelController.LoadData();
                Points = _modelController.GlucosePoints;
                LoadText = "Data loaded.";

            });

            AddCommand = new RelayCommand(() =>
            {
                Random r = new Random();
                var date = DateTime.Now;
                date = date.AddDays((int) (r.NextDouble() > 0.5 ? r.NextDouble() * 10 : r.NextDouble() * (-10)));
                DataPoint point = new DataPoint(DateTimeAxis.ToDouble(date), 5 + r.NextDouble() * 5);
                Points.Add(point);
                AddText = "Point added. ";

                if (_modelController.GlucosePoints.Contains(point))
                {
                    AddText += "Collection contains point. ";
                    var glucose = _modelController.GlucoseDatapoints[point];
                    AddText += glucose?.ToString() ?? "Glucose not generated.";
                }
                else
                {
                    AddText += "Collection does not contain point";
                }
            });

            RemoveCommand = new RelayCommand(() =>
            {
                var point = Points[0];
                if (Points.Remove(point))
                {
                    RemoveText = "Point deleted. ";
                    RemoveText += _modelController.GlucosePoints.Contains(point)
                        ? "Point still exists in collection. "
                        : "Point removed from collection. ";
                    RemoveText += _modelController.GlucoseDatapoints.Keys.Contains(point)
                        ? "Point still in dictionary"
                        : "Point removed from dictionary";
                }
                else
                {
                    RemoveText = "Point NOT deleted. ";
                }
            });

            UpdateCommand = new RelayCommand(() =>
            {
                var oldPoint = Points[0];
                var newPoint = new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 7.8);
                Points[0] = newPoint;
                UpdateText = "Point replaced.";
                UpdateText += _modelController.GlucosePoints.Contains(oldPoint)
                    ? "Point was NOT removed from collection. "
                    : "Point was removed from collection. ";
                UpdateText += _modelController.GlucosePoints.Contains(newPoint)
                    ? "New point added. "
                    : "New point missing. ";
                UpdateText += _modelController.GlucoseDatapoints.Keys.Contains(oldPoint)
                    ? "Point NOT removed from dictionary. "
                    : "Point removed from dictionary. ";
                UpdateText += _modelController.GlucoseDatapoints.Keys.Contains(newPoint)
                    ? "Point added to dictionary. "
                    : "Point NOT added to dictionary. ";
                var glucose = _modelController.GlucoseDatapoints[newPoint];
                UpdateText += glucose.ToString();


            });

            SortCommand = new RelayCommand(() => Points.Sort(point => point.X));
        }


        public GraphViewModel()
        {
            Points = new ObservableCollection<DataPoint>();
        }

        public GraphViewModel(PlotModel model) : this()
        {
            Model = model;
        }

        public GraphViewModel(PlotModel model, ModelController modelController) : this()
        {
            Model = model;
            _modelController = modelController;
            Points = _modelController.GlucosePoints;
            Points.Sort(point => point.X);
            SetCommands();
        }

        public GraphViewModel(PlotModel model, ObservableCollection<DataPoint> points)
        {
            Model = model;
            Points = points;
        }


        private void AddPoint(DataPoint point)
        {
            Application.Current.Dispatcher?.Invoke(() => Points.Add(point));
        }

        private void SetPoints(ObservableCollection<DataPoint> points)
        {
            Application.Current.Dispatcher?.Invoke(() => Points = points);
        }

        public void Refresh()
        {
            Points.Sort(point => point.X);
        }
    }
}
