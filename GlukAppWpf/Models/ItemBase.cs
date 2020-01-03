using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GlukAppWpf.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

public delegate void PropertyChanging(object sender, OnPropertyChangingEventArgs args);

namespace GlukAppWpf.Models
{

    public class ItemBase : ObservableObject
    {
        private PropertyChanging PropertyChanging;
        public event PropertyChanging OnPropertyChanging;


        private DataPoint _point;
        private DataPoint Point
        {
            get => _point;
            set
            {
                RaisePropertyChanging(this, new OnPropertyChangingEventArgs()
                {
                    propertyName = nameof(Point),
                    OldValue = _point,
                    Value = value
                });
                _point = value;
                //RaisePropertyChanged(() => Point);
            }
        }

        private ScatterPoint _highlightPoint;
        private ScatterPoint HighlightPoint
        {
            get => _highlightPoint;
            set
            {
                RaisePropertyChanging(this, new OnPropertyChangingEventArgs()
                {
                    propertyName = nameof(HighlightPoint),
                    OldValue = _highlightPoint,
                    Value = value
                });
                _highlightPoint = value;
                //RaisePropertyChanged(() => HighlightPoint);
            }
        }

        protected int _id;

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                InitializePoints();
                RaisePropertyChanged(() => Date);
                
            } }

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                InitializePoints();
                RaisePropertyChanged(() => Value);
                
            }
        }

        public ItemBase()
        {
            _id = -1;
            _point = new DataPoint();
        }

        public ItemBase(DateTime date, float value) : this()
        {
            Date = date;
            Value = value;
        }

        public ItemBase(int id, DateTime date, float value)
        {
            _id = id;
            Date = date;
            Value = value;
        }

        public int GetId()
        {
            return _id;
        }

        public DataPoint GetDataPoint()
        {
            return Point;
        }

        public ScatterPoint GetScatterPoint()
        {
            return HighlightPoint;
        }

        public void InitializePoints()
        {
            Point = new DataPoint(DateTimeAxis.ToDouble(Date), Value);
            HighlightPoint = new ScatterPoint(_point.X, _point.Y);
        }

        private void RaisePropertyChanging(object sender, OnPropertyChangingEventArgs args)
        {
            OnPropertyChanging?.Invoke(sender, args);
        }

        public override bool Equals(object obj)
        {
            if (obj is ItemBase item)
            {
                return item.GetId() == _id
                       && item.Date.Equals(Date)
                       && Math.Abs(item.Value - Value) < 0.095;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (7 * hash) + _id.GetHashCode();
            hash = (7 * hash) + Date.GetHashCode();
            hash = (7 * hash) + Value.GetHashCode();
            return hash;
        }

        
    }

    public class OnPropertyChangingEventArgs
    {
        public string propertyName { get; set; }
        public object OldValue { get; set; }
        public object Value { get; set; }
    }
}
