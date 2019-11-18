using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NLog;

namespace GlukLibrary
{
    public static class HelperMethods
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

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
            for (int i = 0; i < a.Count; i++)
            {
                observableCollection.Move(observableCollection.IndexOf(a[i]), i);
            }
        }

        public static bool WaitForFile(string fullPath, int count)
        {
            int numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    // Attempt to open the file exclusively.
                    using (FileStream fs = new FileStream(fullPath,
                        FileMode.Open, FileAccess.ReadWrite,
                        FileShare.None, 100))
                    {
                        fs.ReadByte();

                        // If we got this far the file is ready
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LOG.Warn(
                        "WaitForFile {0} failed to get an exclusive lock: {1}",
                        fullPath, ex.ToString());

                    if (numTries > count)
                    {
                        LOG.Warn(
                            "WaitForFile {0} giving up after 10 tries",
                            fullPath);
                        return false;
                    }

                    // Wait for the lock to be released
                    System.Threading.Thread.Sleep(100);
                }
            }

            LOG.Info("WaitForFile {0} returning true after {1} tries",
                fullPath, numTries);
            return true;
        }
    }
}
