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


        //DEBUG PROPERTIES
        private Stack<DataPoint> _stack;
        private int _range = 50;


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

            //DEBUG CODE
            _stack = new Stack<DataPoint>();
            List<DataPoint> MyPoints = new List<DataPoint>();
            MyPoints.Add(new DataPoint(0, 1));
            for (int i = 1; i < _range; i++)
            {
                MyPoints.Add(new DataPoint(i, (i - MyPoints[i - 1].Y) % 13));
            }

            SetPoints(new ObservableCollection<DataPoint>(MyPoints));

            MyPoints.Reverse();
            foreach (var point in MyPoints)
            {
                _stack.Push(point);
            }
            Timer timer  = new Timer(200);
            timer.Elapsed += (sender, args) =>
            {

                if (_stack.Count > 0)
                {
                    DataPoint p = _stack.Pop();
                    AddPoint(new DataPoint(p.X + _range, p.Y));
                }
                else
                {
                    timer.Stop();
                }
            };
            timer.Start();

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
