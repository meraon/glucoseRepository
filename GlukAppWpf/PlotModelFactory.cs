using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GlukAppWpf
{
    public class PlotModelFactory
    {
        private PlotModelFactory()
        {
        }

        public static PlotModel LineSeriesModel()
        {
            PlotModel model = new PlotModel();
            DateTimeAxis xAxis = new DateTimeAxis();
            xAxis.Position = AxisPosition.Bottom;
            model.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis();
            yAxis.Position = AxisPosition.Left;
            model.Axes.Add(yAxis);
            return model;

            
        }

        public static void CreateSeries(PlotModel model, IList<DataPoint> points)
        {
            LineSeries serie = new LineSeries();
            serie.ItemsSource = points;
            model.Series.Add(serie);


        }
    }
}
