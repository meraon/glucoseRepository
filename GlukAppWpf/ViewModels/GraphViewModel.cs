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
using GlukAppWpf.Annotations;
using OxyPlot;
using OxyPlot.Wpf;
using PlotCommands = OxyPlot.Wpf.PlotCommands;
using Timer = System.Timers.Timer;

namespace GlukAppWpf.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
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
       
    }
}
