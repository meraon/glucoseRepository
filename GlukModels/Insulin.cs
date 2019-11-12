using System;
using Newtonsoft.Json;

namespace GlukLibrary
{
    public class Insulin : EntryModel
    {
        public static readonly string VALUE_FORMAT = "#";

        [JsonProperty("isDayDosage")]
        private bool _isDayDosage;

        public Insulin()
        {
            setTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            setValue(0f);
            setIsDayDosage(true);
        }

        public Insulin(long timestamp, float value, bool isDayDosage)
        {
            setTimestamp(timestamp);
            setValue(value);
            this._isDayDosage = isDayDosage;
        }

        public Insulin(int id, long timestamp, float value, bool isDayDosage)
        {
            setId(id);
            setTimestamp(timestamp);
            setValue(value);
            this._isDayDosage = isDayDosage;
        }

        public bool isIsDayDosage()
        {
            return _isDayDosage;
        }

        public void setIsDayDosage(bool isDayDosage)
        {
            this._isDayDosage = isDayDosage;
        }

        public override bool Equals(object obj)
        {
            if (obj is Insulin item)
            {
                return item.getId() == Id
                       && item.getTimestamp() == Timestamp
                       && item.getValue() == Value
                       && item.isIsDayDosage() == _isDayDosage;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = (7 * hash) + _isDayDosage.GetHashCode();
            return hash;
        }
    }
}
