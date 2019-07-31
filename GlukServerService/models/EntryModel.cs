using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlukServerService.models
{
    public abstract class EntryModel
    {
        public static readonly string DATE_TIME_FORMAT = "yyyy/MM/dd HH:mm";
        public static readonly string DATE_FORMAT = "yyyy/MM/dd";
        public static readonly string TIME_FORMAT = "HH:mm";

        protected int id;
        protected long timestamp;
        protected float value;

        public EntryModel()
        {
        }

        public int getId()
        {
            return id;
        }

        public void setId(int id)
        {
            this.id = id;
        }

        public long getTimestamp()
        {
            return timestamp;
        }

        public void setTimestamp(long timestamp)
        {
            this.timestamp = timestamp;
        }

        public float getValue()
        {
            return value;
        }

        public void setValue(float value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
