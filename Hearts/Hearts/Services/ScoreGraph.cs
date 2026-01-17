using Hearts.Models;
using System.Collections.ObjectModel;

namespace Hearts.Services
{
    public class ScoreGraph : IDrawable
    {
        // Players to draw. The page will assign this from the GameService/ViewModel.
        public ObservableCollection<Player>? Players { get; set; }
        // Threshold line to show on graph (set by page)
        public int Threshold { get; set; }

        // Provide a slightly desaturated/soft palette
        private static string GetSoftColorHex(int index)
        {
            // Soft palette hex values
            var colors = new[] { "#ff7b7b", "#7bd1ff", "#9eff7b", "#ffb37b" };
            return colors[index % colors.Length];
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (Players == null || Players.Count == 0)
                return;

            float padding = 20;
            float width = Math.Max(0, dirtyRect.Width - padding * 2);
            float height = Math.Max(0, dirtyRect.Height - padding * 2);

            // Fill background a darker gray for contrast
            canvas.FillColor = Color.FromArgb("#2b2b2b");
            canvas.FillRectangle(dirtyRect);

            // Determine number of rounds (max number of entries any player has)
            int maxRounds = Players.Max(p => p.CumulativeScores.Count);

            // Compute cumulative totals for each player and the overall max
            var cumulativeLists = new List<List<int>>();
            int overallMax = 0;

            foreach (var p in Players)
            {
                var cum = new List<int>();
                int running = 0;
                foreach (var v in p.CumulativeScores)
                {
                    running += v;
                    cum.Add(running);
                }
                if (cum.Count > 0)
                    overallMax = Math.Max(overallMax, cum.Max());
                cumulativeLists.Add(cum);
            }

            // Allow drawing even when there is a single round or zero scores
            if (maxRounds < 1)
                maxRounds = 1;

            // Ensure threshold appears on the chart and scale so threshold sits ~4/5 up the graph
            overallMax = Math.Max(1, overallMax);
            if (Threshold > 0)
            {
                // desired Threshold/overallMax = 0.8 => overallMax = Threshold / 0.8 = Threshold * 1.25
                var minForThreshold = (int)Math.Ceiling(Threshold * 1.25);
                overallMax = Math.Max(overallMax, minForThreshold);
            }

            canvas.StrokeSize = 2;

            // simple color palette
            var palette = new Color[] { Colors.Red, Colors.Cyan, Colors.Lime, Colors.Orange };

            // axis coordinates
            float axisLeftX = padding;
            float axisBottomY = padding + height;

            // Choose a sensible number of Y ticks ahead of drawing the grid.
            // If the number of rounds is small, prefer splitting by rounds; otherwise cap ticks.
            int yTicksCap = Math.Min(8, Math.Max(4, overallMax));
            int yTicksActual = maxRounds <= 8 ? maxRounds : (overallMax <= yTicksCap ? overallMax : yTicksCap);
            if (yTicksActual < 1) yTicksActual = 1;

            // Draw grid behind lines (very soft, low-contrast)
            canvas.StrokeSize = 1;
            // horizontal grid lines
            canvas.FontSize = 12;
            var gridColor = Color.FromRgba(1f, 1f, 1f, 0.02f);
            for (int t = 0; t <= yTicksActual; t++)
            {
                float value = t * (overallMax / (float)yTicksActual);
                float y = padding + height - (value / (float)overallMax) * height;
                canvas.StrokeColor = gridColor;
                canvas.DrawLine(axisLeftX, y, axisLeftX + width, y);
            }

            // vertical grid lines removed (round-based vertical grid disabled)

            // Draw axes on top of grid
            canvas.StrokeColor = Colors.LightGray;
            canvas.DrawLine(axisLeftX, padding, axisLeftX, axisBottomY);
            canvas.DrawLine(axisLeftX, axisBottomY, axisLeftX + width, axisBottomY);

            // Draw threshold dashed line across the graph if Threshold > 0
            if (Threshold > 0)
            {
                float threshY = padding + height - (Threshold / (float)overallMax) * height;
                float dash = 8;
                float gap = 6;
                float x = axisLeftX;
                canvas.StrokeSize = 1.5f;
                canvas.StrokeColor = Colors.DarkRed;
                while (x < axisLeftX + width)
                {
                    float segEnd = Math.Min(axisLeftX + width, x + dash);
                    canvas.DrawLine(x, threshY, segEnd, threshY);
                    x = segEnd + gap;
                }

                // label the threshold on the right side
                string tlabel = "";
                canvas.FontSize = 12;
                canvas.FontColor = Colors.DarkRed;
                canvas.DrawString(tlabel, new RectF(axisLeftX + width + 6, threshY - 8, 80, 16), HorizontalAlignment.Left, VerticalAlignment.Top);
            }

            // Now draw player lines on top of the grid
            for (int pi = 0; pi < cumulativeLists.Count; pi++)
            {
                var cum = cumulativeLists[pi];
                // If there's only one data point, draw a horizontal line spanning the graph at the score value
                if (cum.Count == 1)
                {
                    float y = padding + height - (cum[0] / (float)overallMax) * height;
                    float xStart = padding;
                    float xEnd = padding + width;
                    canvas.StrokeColor = Color.FromArgb(GetSoftColorHex(pi));
                    canvas.StrokeSize = 4.5f;
                    canvas.DrawLine(xStart, y, xEnd, y);
                    // draw end markers
                    canvas.FillColor = Color.FromArgb(GetSoftColorHex(pi));
                    canvas.FillCircle(xStart, y, 4);
                    canvas.FillCircle(xEnd, y, 4);
                }
                else if (cum.Count > 1)
                {
                    var path = new PathF();

                    for (int i = 0; i < cum.Count; i++)
                    {
                        float denom = Math.Max(1, maxRounds - 1);
                        float x = padding + (i / (float)denom) * width;
                        float y = padding + height - (cum[i] / (float)overallMax) * height;

                        if (i == 0)
                            path.MoveTo(x, y);
                        else
                            path.LineTo(x, y);
                    }

                    canvas.StrokeColor = Color.FromArgb(GetSoftColorHex(pi));
                    canvas.StrokeSize = 4.5f;
                    canvas.DrawPath(path);
                }
            }

            

            // (axes and threshold were already drawn before plotting)
            // Draw small Y-axis ticks only (no numeric labels)
            canvas.StrokeColor = Colors.LightGray;
            for (int t = 0; t <= yTicksActual; t++)
            {
                float value = t * (overallMax / (float)yTicksActual);
                float y = padding + height - (value / (float)overallMax) * height;
                canvas.DrawLine(axisLeftX - 6, y, axisLeftX, y);
            }

            // Draw vertical grid lines and round labels on X axis
            for (int i = 0; i < maxRounds; i++)
            {
                float denom = Math.Max(1, maxRounds - 1);
                float x = padding + (i / (float)denom) * width;
                // small tick
                canvas.DrawLine(x, axisBottomY, x, axisBottomY + 6);

                // vertical grid lines removed (no-op)

                // label (round numbers start at 1) - use cream color for readability
                canvas.FontColor = Color.FromArgb("#f5e9d3");
                canvas.DrawString((i + 1).ToString(), new RectF(x - 12, axisBottomY + 6, 24, 16), HorizontalAlignment.Center, VerticalAlignment.Top);
            }

            // Draw legend at top-left with border and background
            float legendX = axisLeftX + 6;
            float legendY = padding - 12;
            float legendWidth = 180;
            float legendHeight = Math.Max(Players.Count * 18 + 8, 28);
            // background
            canvas.FillColor = Color.FromArgb("#1f1f1f");
            canvas.FillRectangle(new RectF(legendX, legendY, legendWidth, legendHeight));
            // border
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;
            canvas.DrawRectangle(new RectF(legendX, legendY, legendWidth, legendHeight));

            // legend items with current totals
            for (int pi = 0; pi < Players.Count; pi++)
            {
                var name = string.IsNullOrWhiteSpace(Players[pi].Name) ? $"Player {pi + 1}" : Players[pi].Name;
                var total = Players[pi].CumulativeScores.Sum();
                // marker
                canvas.FillColor = Color.FromArgb(GetSoftColorHex(pi));
                canvas.FillRectangle(legendX + 8, legendY + 6 + pi * 18, 12, 12);
                // text
                canvas.FontColor = Colors.White;
                canvas.FontSize = 12;
                canvas.DrawString($"{name} - {total}", new RectF(legendX + 28, legendY + 6 + pi * 18, legendWidth - 36, 16), HorizontalAlignment.Left, VerticalAlignment.Top);
            }

            // Axis labels
            // X label - use an inverse/light color of the background so it isn't red
            canvas.FontColor = Color.FromArgb("#d4d4d4");
            canvas.DrawString("Round", new RectF(axisLeftX + width / 2 - 40, axisBottomY + 28, 80, 20), HorizontalAlignment.Center, VerticalAlignment.Top);

            // Y label (vertical stacked characters)
            string yLabel = "Score";
            float labelFontSize = 12;
            canvas.FontSize = labelFontSize;
            // center vertically alongside Y axis
            float totalHeight = yLabel.Length * labelFontSize;
            float startY = padding + height / 2 - (totalHeight / 2);
            // Y label in cream for good contrast against the grey background
            canvas.FontColor = Color.FromArgb("#f5e9d3");
            for (int i = 0; i < yLabel.Length; i++)
            {
                canvas.DrawString(yLabel[i].ToString(), new RectF(axisLeftX - 56, startY + i * labelFontSize, 40, labelFontSize), HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }
    }
}
