using System;
using Newtonsoft.Json;

namespace GlukModels
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
            return base.Equals(obj);
        }
    }
}
