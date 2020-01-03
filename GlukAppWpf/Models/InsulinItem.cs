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

        public InsulinItem()
        {
        }

        public InsulinItem(DateTime date, float value, bool isDayDosage) : base(date, value)
        {
            IsDayDosage = isDayDosage;
            InitializePoints();
        }

        public InsulinItem(int id, DateTime date, float value, bool isDayDosage) : base(id, date, value)
        {
            IsDayDosage = isDayDosage;
            InitializePoints();
        }

        public override bool Equals(object obj)
        {
            if (obj is InsulinItem item)
            {
                return item.GetId() == _id
                       && item.Date.Equals(Date)
                       && Math.Abs(item.Value - Value) < 0.095
                       && item.IsDayDosage == IsDayDosage;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = (7 * hash) + IsDayDosage.GetHashCode();
            return hash;
        }
    }
}
