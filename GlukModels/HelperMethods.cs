using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GlukModels
{
    public static class HelperMethods
    {
        public static long DateTimeStringToTimestamp(string dt)
        {
            var dateTime = DateTime.Parse(dt);
            var totalSeconds = (long)dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return totalSeconds;
        }

        public static long DateTimeToTimestamp(DateTime dt)
        {
            var totalSeconds = (long)dt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return totalSeconds;
        }

        public static DateTime TimestampToDateTime(long timestamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(timestamp);
            return dt;
        }

        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> observableCollection, Func<TSource, TKey> keySelector)
        {
            var a = observableCollection.OrderBy(keySelector).ToList();
            observableCollection.Clear();
            foreach (var b in a)
            {
                observableCollection.Add(b);
            }
        }
    }
}
