using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlukAppWpf.Models
{
    public class GlucoseItem : ItemBase
    {
        public GlucoseItem()
        {
        }

        public GlucoseItem(DateTime date, float value) : base(date, value)
        {
            GenerateDataPoint();
        }

        public GlucoseItem(int id, DateTime date, float value) : base(id, date, value)
        {
            GenerateDataPoint();
        }
    }
}
