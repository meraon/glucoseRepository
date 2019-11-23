using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlukAppWpf.ViewModels;
using OxyPlot;

namespace GlukAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        public TablePage()
        {
            InitializeComponent();
        }

        public TablePage(ModelProvider modelController, DataSource source) : this()
        {
            DataContext = new TableViewModel(DataGrid, modelController, source);
        }
    }
}
