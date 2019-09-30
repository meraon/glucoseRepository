using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GlukServerService.models
{
    public class Insulin : EntryModel
    {
        public static readonly string VALUE_FORMAT = "#";

        [JsonProperty("isDayDosage")]
        private bool isDayDosage;

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
            this.isDayDosage = isDayDosage;
        }

        public Insulin(int id, long timestamp, float value, bool isDayDosage)
        {
            setId(id);
            setTimestamp(timestamp);
            setValue(value);
            this.isDayDosage = isDayDosage;
        }

        public bool isIsDayDosage()
        {
            return isDayDosage;
        }

        public void setIsDayDosage(bool isDayDosage)
        {
            this.isDayDosage = isDayDosage;
        }
    }
}
