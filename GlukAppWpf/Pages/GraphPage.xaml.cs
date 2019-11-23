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
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GlukAppWpf.ViewModels;
using OxyPlot;
using OxyPlot.Series;

namespace GlukAppWpf.Pages
{
    /// <summary>
    /// Interaction logic for GraphPage.xaml
    /// </summary>
    public partial class GraphPage : Page
    {
        private GraphViewModel _context;


        public GraphPage()
        {
            InitializeComponent();
            _context = new GraphViewModel(Plot.ActualModel);
            DataContext = _context;
        }

        public GraphPage(ModelProvider modelController)
        {
            InitializeComponent();
            DataContext = new GraphViewModel(Plot.ActualModel, modelController);
        }

        public GraphPage(ModelProvider modelController, DataSource source)
        {
            InitializeComponent();
            DataContext = new GraphViewModel(Plot.ActualModel, modelController, source);
        }

        public GraphPage(ObservableCollection<DataPoint> points)
        {
            InitializeComponent();
            DataContext = new GraphViewModel(Plot.ActualModel, points);
        }
    }
}
