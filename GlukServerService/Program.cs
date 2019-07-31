using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GlukServerService
{
    static class Program
    {
        public static CultureInfo CULTURE = new CultureInfo("en-US", false);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            SetCulture(CULTURE);

            var servicesToRun = new ServiceBase[]
            {
                new GlukServerService()
            };
            ServiceBase.Run(servicesToRun);
        }

        public static void SetCulture(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
        }
    }
}
