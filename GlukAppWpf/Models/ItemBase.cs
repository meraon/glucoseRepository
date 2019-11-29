using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GlukAppWpf.Models
{
    public class ItemBase : ObservableObject
    {
        private DataPoint _point;
        private ScatterPoint _highlightPoint;

        protected int _id;
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            } }

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
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
            return _point;
        }

        public ScatterPoint GetScatterPoint()
        {
            return _highlightPoint;
        }

        /// <summary>
        /// Generates new datapoint based on current Date and Value
        /// </summary>
        /// <returns>Newly-generated point</returns>
        public DataPoint GenerateDataPoint()
        {
            _point = new DataPoint(DateTimeAxis.ToDouble(Date), Value);
            _highlightPoint = new ScatterPoint(_point.X, _point.Y);
            return _point;
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
}
