using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GlukServerService;
using GlukServerService.models;
using GlukServerService.network;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            string registryPath = @"System\CurrentControlSet\services\GlukServerService";

            RegistryKey registry = Registry.CurrentUser.OpenSubKey(registryPath, true);
            if (registry != null)
            {
                Console.WriteLine("OK");
            }
            else
            {
                RegistryKey regKey = Registry.CurrentUser.CreateSubKey(registryPath);
                regKey.SetValue("backupPath",@"D:\dia_data_backup.sql");
                regKey.SetValue("restoreDbOnStartUp", @"false");
                regKey.SetValue("testBackup", @"false");
            }

            Console.ReadLine();


        }
    }
}
