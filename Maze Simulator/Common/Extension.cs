using System;
using System.Windows.Media;

namespace Maze_Simulator.Common
{
    public static class Extension
    {
        public static Color ColorFromHSV(double h, double s, double v)
        {
            double c = s * v;
            double x = c * (1 - Math.Abs((h / 60 % 2) - 1));
            var (r, g, b) = h switch
            {
                >= 0 and < 60 => (c, x, 0.0),
                >= 60 and < 120 => (x, c, 0.0),
                >= 120 and < 180 => (0.0, c, x),
                >= 180 and < 240 => (0.0, x, c),
                >= 240 and < 300 => (x, 0.0, c),
                >= 300 and < 360 => (c, 0.0, x),
                _ => throw new ArgumentOutOfRangeException()
            };

            double m = v - c;
            byte R = (byte)((r + m) * 255);
            byte G = (byte)((g + m) * 255);
            byte B = (byte)((b + m) * 255);
            return Color.FromRgb(R, G, B);
        }
    }
}
