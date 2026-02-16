using System.Drawing;
using System.Drawing.Imaging;
using ScottPlot;

namespace XCoinMonthChart.Services;

public class ChartRenderer
{
    public byte[] RenderMonthlyAverages(Dictionary<int, Dictionary<int, double>> data)
    {
        if (data == null || data.Count == 0)
        {
            // produce a small placeholder PNG so the UI shows a clear message
            using var bmpPlaceholder = new Bitmap(800, 200);
            using var g = Graphics.FromImage(bmpPlaceholder);
            g.Clear(Color.White);
            using var font = new Font("Segoe UI", 14);
            using var brush = new SolidBrush(Color.Black);
            var msg = "No data available.";
            g.DrawString(msg, font, brush, new PointF(12, 80));
            using var msP = new MemoryStream();
            bmpPlaceholder.Save(msP, ImageFormat.Png);
            return msP.ToArray();
        }

        // Collect all data points
        var points = new List<(DateTime date, double price)>();
        foreach (var yearKv in data)
        {
            int year = yearKv.Key;
            foreach (var monthKv in yearKv.Value)
            {
                int month = monthKv.Key;
                double avg = monthKv.Value;
                points.Add((new DateTime(year, month, 1), avg));
            }
        }
        points = points.OrderBy(p => p.date).ToList();

        if (points.Count == 0)
        {
            // no points
            using var bmpPlaceholder = new Bitmap(800, 200);
            using var g = Graphics.FromImage(bmpPlaceholder);
            g.Clear(Color.White);
            using var font = new Font("Segoe UI", 14);
            using var brush = new SolidBrush(Color.Black);
            var msg = "No data points available.";
            g.DrawString(msg, font, brush, new PointF(12, 80));
            using var msP = new MemoryStream();
            bmpPlaceholder.Save(msP, ImageFormat.Png);
            return msP.ToArray();
        }

        // larger canvas for clarity
        var plt = new ScottPlot.Plot(1200, 480);
        plt.Style(figureBackground: Color.White, dataBackground: Color.White);

        var colors = new[] { Color.Red, Color.Blue, Color.Yellow, Color.Green, Color.Purple, Color.Orange };
        int colorIndex = 0;

        foreach (var yearKv in data.OrderBy(kv => kv.Key))
        {
            int year = yearKv.Key;
            var yearPoints = new List<(DateTime date, double price)>();
            foreach (var monthKv in yearKv.Value.OrderBy(kv => kv.Key))
            {
                int month = monthKv.Key;
                double avg = monthKv.Value;
                yearPoints.Add((new DateTime(year, month, 1), avg));
            }

            if (yearPoints.Count > 0)
            {
                double[] xs = yearPoints.Select(p => p.date.ToOADate()).ToArray();
                double[] ys = yearPoints.Select(p => p.price).ToArray();
                var color = colors[colorIndex % colors.Length];
                plt.AddScatter(xs, ys, color: color, markerSize: 0, lineWidth: 2, label: year.ToString());
                colorIndex++;
            }
        }

        // grid, labels
        plt.Grid(enable: true, lineStyle: ScottPlot.LineStyle.Solid, color: Color.LightGray);

        plt.XLabel("Date");
        plt.YLabel("Average Price (USD)");
        plt.Layout(right: 0.82f, left: 60, top: 60);

        plt.Title("Bitcoin Monthly Average Prices by Year");

        // Format x-axis as dates
        plt.XAxis.DateTimeFormat(true);

        // Add legend
        plt.Legend();

        using var bmp = plt.Render();
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
