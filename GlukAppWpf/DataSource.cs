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
        private DataSources _source;
        public DataSources Source
        {
            get => _source;
            set
            {
                _source = value;
                RaisePropertyChanged(() => Source);
            }
        }

        public DataSource()
        {
            Source = DataSources.Glucoses;
        }

        public DataSource(DataSources source)
        {
            Source = source;
        }
    }

    public enum DataSources
    {
        Glucoses,
        Insulins
    }
}
