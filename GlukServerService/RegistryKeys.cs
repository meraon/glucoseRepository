using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlukServerService
{
    public static class RegistryKeys
    {
        public static string RegistryPath = @"System\CurrentControlSet\services\GlukServerService";

        public static readonly string BackupPath = "backupPath";
        public static readonly string RestoreDbOnStartUp = "restoreDbOnStartUp";
        public static readonly string TestBackup = "testBackup";
    }
}
