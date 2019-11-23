using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlukAppWpf.Models
{
    public class ItemBase
    {
        protected int _id;
        public DateTime Date { get; set; }
        public float Value { get; set; }

        public ItemBase()
        {
            _id = -1;
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
