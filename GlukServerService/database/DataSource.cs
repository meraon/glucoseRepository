using System.Collections.Generic;
using GlukModels;

namespace GlukServerService
{
    public interface DataSource
    {
        void SaveGlucoses(List<Glucose> items);
        void SaveInsulins(List<Insulin> items);
        List<Glucose> GetGlucoses();
        List<Insulin> GetInsulins();
        void MakeBackup(string file);
        void RestoreBackup(string file);
    }
}