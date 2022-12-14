using Microsoft.Maui.Graphics;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace TiltViewer.View
{
    public class ChartDrawable : ReactiveObject, IDrawable
    {
        private const float _axisLabelOffset = 8;
        private Color _axisColor = Color.FromArgb("2DFFFFFF");

        private readonly LinearGradientPaint _chartGradient = new LinearGradientPaint
        {
            StartColor = Color.FromArgb("FF36612A"),
            EndColor = Color.FromArgb("0036612A"),
            EndPoint = new Point(0, 1)
        };

        private ObservableCollection<Tuple<DateTime, float>> _data;
        public ObservableCollection<Tuple<DateTime, float>> Data
        {
            get => _data;
            private set => this.RaiseAndSetIfChanged(ref _data, value);
        }

        private int _chartHeight = 80;
        public int ChartHeight
        {
            get => _chartHeight;
            set => this.RaiseAndSetIfChanged(ref _chartHeight, value);
        }

        private int _chartWidth = 210;
        public int ChartWidth
        {
            get => _chartWidth;
            set => this.RaiseAndSetIfChanged(ref _chartWidth, value);
        }

        public ChartDrawable()
        {
            // Some fake data...
            Data = new ObservableCollection<Tuple<DateTime, float>>
            {
                new (DateTime.Parse("12/4/2022 14:18:45"), 1.031f),
                new (DateTime.Parse("12/4/2022 16:46:09"), 1.028f),
                new (DateTime.Parse("12/4/2022 17:14:54"), 1.027f),
                new (DateTime.Parse("12/4/2022 21:12:13"), 1.021f),
                new (DateTime.Parse("12/5/2022 9:06:31"), 1.015f),
                new (DateTime.Parse("12/6/2022 8:44:27"), 1.01f),
                new (DateTime.Parse("12/7/2022 13:16:18"), 1.01f),
                new (DateTime.Parse("12/9/2022 10:51:19"), 1.01f),
                new (DateTime.Parse("12/9/2022 11:55:28"), 1.01f)
            };
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IEnumerable<DateTime> xAxisDataValues = _data.Select(x => x.Item1);
            IEnumerable<float> yAxisDataValues = _data.Select(x => x.Item2);

            canvas.Translate(0, _axisLabelOffset);

            // Draw axis lines
            canvas.StrokeColor = _axisColor;
            canvas.StrokeSize = 1;

            // Not technically correct, but good enough
            // Chart each day as an axis line
            var yAxisLines = GetYAxisPoints(_data.Select(x => x.Item1));
            for (int i = 0; i < yAxisLines.Count; i++)
            {
                canvas.DrawLine(yAxisLines[i], 0, yAxisLines[i], _chartHeight);
            }

            // Scale plot values to fit in the available space on the control
            var scaledX = ScaleDateTimeToLinearSpace(xAxisDataValues, _chartWidth);
            var scaledY = ScaleToLinearSpace(yAxisDataValues, _chartHeight);

            PathF path = new();
            for (int i = 0; i < scaledX.Count; i++)
            {
                path.LineTo((int)scaledX[i], (int)scaledY[i]);
            }
            path.LineTo((int)scaledX[scaledX.Count - 1], _chartHeight);
            path.LineTo((int)scaledX[0], _chartHeight);
            path.Close();

            canvas.StrokeColor = _axisColor;
            canvas.DrawRectangle(0, 0, _chartWidth, _chartHeight);

            canvas.StrokeColor = Color.FromArgb("82BA77");
            canvas.StrokeSize = 1;
            canvas.StrokeLineJoin = LineJoin.Round;
            canvas.SetFillPaint(_chartGradient, RectF.Zero);

            // Draw the lines
            canvas.FillPath(path);
            canvas.DrawPath(path);

            // Draw each data point
            for (int i = 0; i < scaledX.Count; i++)
            {
                canvas.DrawEllipse((int)scaledX[i] - 2, (int)scaledY[i] - 2, 4, 4);
            }

            // Draw the min and max axis labels
            float padding = (yAxisDataValues.Max() - yAxisDataValues.Min()) * 0.2f;
            float lowerAxisLabel = yAxisDataValues.Min() - padding;
            float upperAxisLabel = yAxisDataValues.Max() + padding;

            canvas.Translate(0, -_axisLabelOffset);
            canvas.FontColor = _axisColor;
            canvas.DrawString(lowerAxisLabel.ToString("F3"), _chartWidth + 4, _chartHeight + 10, HorizontalAlignment.Left);
            canvas.DrawString(upperAxisLabel.ToString("F3"), _chartWidth + 4, 10, HorizontalAlignment.Left);
        }

        private List<float> GetYAxisPoints(IEnumerable<DateTime> timeRange)
        {
            List<float> axisPoints = new();

            var range = timeRange.Max() - timeRange.Min();
            for (int i = 0; i < Math.Ceiling(range.TotalDays); i++)
            {
                axisPoints.Add(i);
            }

            return ScaleToLinearSpace(axisPoints, _chartWidth);
        }

        private static List<float> ScaleToLinearSpace(IEnumerable<float> inputList, float scale)
        {
            List<float> outputList = new();

            float diff = inputList.Max() - inputList.Min();
            float padding = (inputList.Max() - inputList.Min()) * 0.2f; // 20% padding

            // Scale each value
            foreach (float value in inputList)
            {
                outputList.Add(scale - (value - inputList.Min() + padding) / (diff + (2.0f * padding)) * scale);
            }

            return outputList;
        }

        private static List<float> ScaleDateTimeToLinearSpace(IEnumerable<DateTime> inputList, float scale)
        {
            List<float> outputList = new();

            // Calculate the difference between the minimum and maximum DateTime values in the input list
            TimeSpan diff = inputList.Max() - inputList.Min();

            // Scale each DateTime value in the input list to the corresponding value in the output list
            foreach (DateTime value in inputList)
            {
                outputList.Add((float)(value - inputList.Min()).TotalMilliseconds / (float)diff.TotalMilliseconds * scale);
            }

            return outputList;
        }
    }
}
