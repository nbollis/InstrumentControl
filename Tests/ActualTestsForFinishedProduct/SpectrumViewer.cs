using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using MassSpectrometry;

namespace Tests
{
    public static class SpectrumViewer
    {
        public static List<PlotModel> PlotMsDataScans(List<MsDataScan> dataScans)
        {
            List<PlotModel> result = new();
            foreach (var scan in dataScans)
            {
                result.Add(PlotMSDataScan(scan));
            }
            return result;
        }
        public static PlotModel PlotMSDataScan(MsDataScan dataScan)
        {
            return CreatePlotModel(dataScan.MassSpectrum.XArray, dataScan.MassSpectrum.YArray);
        }
        public static void WritePlotToPng(PlotModel plot, string fileName)
        {
            plot.Background = OxyColors.White;
            var pngExport = new PngExporter() { Width = 600, Height = 400 };
            pngExport.ExportToFile(plot, fileName);
        }
        public static PlotModel CreatePlotModel(double[] xarray, double[] yarray)
        {
            PlotModel model = new PlotModel();

            LineSeries ls = new LineSeries();

            for (int i = 0; i < xarray.Length; i++)
            {
                ls.Points.Add(new DataPoint(xarray[i], yarray[i]));
            }
            model.Series.Add(ls);
            return model;
        }
        public static PlotModel CreatePlotModel(MzSpectrum spectrum)
        {
            return CreatePlotModel(spectrum.XArray, spectrum.YArray);
        }
    }
}
