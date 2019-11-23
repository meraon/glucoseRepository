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
    }
}
