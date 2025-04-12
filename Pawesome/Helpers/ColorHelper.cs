using System.Drawing;

namespace Pawesome.Helpers
{
    public class ColorHelper
    {
        public static string EnhanceColor(string hexColor, int boostPercentage = 20)
        {
            if (string.IsNullOrEmpty(hexColor) || !hexColor.StartsWith("#"))
                return hexColor;

            var color = ColorTranslator.FromHtml(hexColor);
            double h, s, l;
            RgbToHsl(color.R, color.G, color.B, out h, out s, out l);
            
            if (s > 0.8)
            {
                s = 1;
            }
            
            if (l > 0.75)
            {
                l = Math.Max(0.5, l - 0.1);
            }
            else if (s < 0.4 || l > 0.7)
            {
                s = Math.Min(1.0, s + boostPercentage / 100.0);
                l = Math.Max(0.0, l - boostPercentage / 200.0);
            }

            var newColor = HslToRgb(h, s, l);
            return $"#{newColor.R:X2}{newColor.G:X2}{newColor.B:X2}";
        }

        private static void RgbToHsl(int r, int g, int b, out double h, out double s, out double l)
        {
            double rd = r / 255.0, gd = g / 255.0, bd = b / 255.0;
            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            l = (max + min) / 2.0;

            if (max == min)
            {
                h = s = 0;
            }
            else
            {
                double d = max - min;
                s = l > 0.5 ? d / (2.0 - max - min) : d / (max + min);

                if (max == rd)
                    h = (gd - bd) / d + (gd < bd ? 6 : 0);
                else if (max == gd)
                    h = (bd - rd) / d + 2;
                else
                    h = (rd - gd) / d + 4;

                h *= 60;
            }
        }

        private static Color HslToRgb(double h, double s, double l)
        {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;

            double r = 0, g = 0, b = 0;

            if (h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (h < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return Color.FromArgb(
                (int)Math.Round((r + m) * 255),
                (int)Math.Round((g + m) * 255),
                (int)Math.Round((b + m) * 255)
            );
        }
    }
}
