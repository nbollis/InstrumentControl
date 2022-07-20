using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using MassSpectrometry;
using Data;

namespace Tests
{
    public static class SpectrumViewer
    {
        /// <summary>
        /// Creates a list of plotmodels from a list of MSDataScans
        /// </summary>
        /// <param name="dataScans"></param>
        /// <returns></returns>
        public static List<PlotModel> PlotMsDataScans(List<MsDataScan> dataScans)
        {
            List<PlotModel> result = new();
            foreach (var scan in dataScans)
            {
                result.Add(CreatePlotModel(scan));
            }
            return result;
        }

        /// <summary>
        /// Create a single plot model from a SingleScanDataObject
        /// </summary>
        /// <param name="singleScanObject"></param>
        /// <returns></returns>
        public static PlotModel CreatePlotModel(SingleScanDataObject singleScanObject)
        {
            return CreatePlotModel(singleScanObject.XArray, singleScanObject.YArray);
        }

        /// <summary>
        /// Creates a single plot model from a MsDataScan
        /// </summary>
        /// <param name="dataScan"></param>
        /// <returns></returns>
        public static PlotModel CreatePlotModel(MsDataScan dataScan)
        {
            return CreatePlotModel(dataScan.MassSpectrum.XArray, dataScan.MassSpectrum.YArray);
        }
        
        /// <summary>
        /// Creates a single plot model from an array on mz values and an array of intensities
        /// </summary>
        /// <param name="xarray">mz values</param>
        /// <param name="yarray">intensities</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a single plotmodel from a MzSpectrum object
        /// </summary>
        /// <param name="spectrum"></param>
        /// <returns></returns>
        public static PlotModel CreatePlotModel(MzSpectrum spectrum)
        {
            return CreatePlotModel(spectrum.XArray, spectrum.YArray);
        }

        /// <summary>
        /// Exports the plot model to a Png image 
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="fileName"></param>
        public static void WritePlotToPng(PlotModel plot, string fileName)
        {
            plot.Background = OxyColors.White;
            var pngExport = new PngExporter() { Width = 600, Height = 400 };
            pngExport.ExportToFile(plot, fileName);
        }

        /// <summary>
        /// Exports the plot model to a Svg image
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="fileName"></param>
        public static void WritePlotToSvg(PlotModel plot, string fileName)
        {
            plot.Background = OxyColors.White;
            var svgExport = new OxyPlot.SvgExporter();
            svgExport.ExportToFile(plot, fileName);
        }
    }
}
