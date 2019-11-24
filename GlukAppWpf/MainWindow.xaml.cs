using GlukAppWpf.ViewModels;
using System.Windows;

namespace GlukAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(GraphFrame, TableFrame);
        }
    }
}
