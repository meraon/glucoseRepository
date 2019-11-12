using System;
using Newtonsoft.Json;

namespace GlukLibrary
{
    public class EntryModel
    {
        public static readonly string DATE_TIME_FORMAT = "yyyy/MM/dd HH:mm";
        public static readonly string DATE_FORMAT = "yyyy/MM/dd";
        public static readonly string TIME_FORMAT = "HH:mm";

        [JsonProperty("id")]
        protected int Id;
        [JsonProperty("timestamp")]
        protected long Timestamp;
        [JsonProperty("value")]
        protected float Value;

        protected EntryModel()
        {
            Id = -1;
        }

        public int getId()
        {
            return Id;
        }

        public void setId(int id)
        {
            this.Id = id;
        }

        public long getTimestamp()
        {
            return Timestamp;
        }

        public void setTimestamp(long timestamp)
        {
            this.Timestamp = timestamp;
        }

        public void setTimestamp(string dateTime)
        {

        }

        public float getValue()
        {
            return Value;
        }

        public void setValue(float value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is EntryModel item)
            {
                return item.getId() == Id
                       && item.getTimestamp() == Timestamp
                       && Math.Abs(item.getValue() - Value) < 0.095;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (7 * hash) + Id.GetHashCode();
            hash = (7 * hash) + Timestamp.GetHashCode();
            hash = (7 * hash) + Value.GetHashCode();
            return hash;
        }
    }
}
