using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace GlukAppWpf
{
    public class DataSource : ObservableObject
    {
        private DataSources _current;
        public DataSources Current
        {
            get => _current;
            set
            {
                _current = value;
                RaisePropertyChanged(() => Current);
            }
        }

        public DataSource()
        {
            Current = DataSources.Glucoses;
        }

        public DataSource(DataSources current)
        {
            Current = current;
        }
    }

    public enum DataSources
    {
        Glucoses,
        Insulins
    }
}
