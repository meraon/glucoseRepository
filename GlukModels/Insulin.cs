using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GlukModels
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
    }
}
