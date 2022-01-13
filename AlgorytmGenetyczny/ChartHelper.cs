using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace AlgorytmGenetyczny
{
    public class ChartHelper
    {
        private readonly string _chart = "chart.png";
        public void GenerateChart(Chart chart, List<IndividualDto> individualList)
        {
            chart.Series.Clear();
            chart.Titles.Clear();
            var minValue = individualList.OrderBy(x => x.AdaptationFunctionValue).First().AdaptationFunctionValue;
            chart.Titles.Add($"Najniższa wartość: {minValue}");
            var pool = chart.Series.Add("Populacja");
            var best = chart.Series.Add("Najlepszy osobnik");
            pool.MarkerStyle = MarkerStyle.Cross;
            best.MarkerStyle = MarkerStyle.Diamond;

            chart.ChartAreas[0].AxisY.Minimum = -4;
            chart.ChartAreas[0].AxisY.Maximum = 2;
            chart.ChartAreas[0].AxisX.Minimum = -4;
            chart.ChartAreas[0].AxisX.Maximum = 2;
            chart.ChartAreas[0].BackImage = Path.GetFullPath(_chart);
            chart.ChartAreas[0].BackImageWrapMode = ChartImageWrapMode.Scaled;

            var poolXPoints = GetPoints(individualList).XPoints;
            var poolYPoints = GetPoints(individualList).YPoints;

            Draw(pool, poolXPoints.GetRange(1, 8), poolYPoints.GetRange(1, 8), SeriesChartType.Point, 7);
            Draw(best, poolXPoints.GetRange(0, 1), poolYPoints.GetRange(0, 1), SeriesChartType.Point, 7);
        }

        private void Draw(Series series, List<double> xValues, List<double> yValues, SeriesChartType chartType, int size)
        {
            series.ChartType = chartType;
            series.MarkerSize = size;
            for (var i = 0; i < xValues.Count; i++)
                series.Points.AddXY(xValues[i], yValues[i]);
        }

        private static PointsDto GetPoints(List<IndividualDto> individualList)
        {
            var listX = new List<double>();
            var listY = new List<double>();

            foreach (var individual in individualList)
            {
                listX.Add(individual.NewParameterValueList[0]);
                listY.Add(individual.NewParameterValueList[1]);
            }

            return new PointsDto
            {
                XPoints = listX,
                YPoints = listY
            };
        }
    }
}
