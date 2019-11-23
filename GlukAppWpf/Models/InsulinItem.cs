using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlukAppWpf.Models
{
    public class InsulinItem : ItemBase
    {
        public bool IsDayDosage { get; set; }


        public InsulinItem(DateTime date, float value, bool isDayDosage) : base()
        {
            Date = date;
            Value = value;
            IsDayDosage = isDayDosage;
        }

        public InsulinItem(int id, DateTime date, float value, bool isDayDosage)
        {
            _id = id;
            Date = date;
            Value = value;
            IsDayDosage = isDayDosage;
        }
    }
}
