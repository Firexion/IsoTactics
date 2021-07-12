using System;
using UnityEngine;

namespace Utils
{
    public static class Colors
    {
        public static Color orange = ToColor("#ff9f43");
        public static Color yellow = ToColor("#feca57");
        public static Color darkBlue = ToColor("#2e86de");
        public static Color red = ToColor("#ee5253");


        private static Color ToColor(string color)
        {
            if (color.StartsWith("#", StringComparison.InvariantCulture))
            {
                color = color.Substring(1); // strip #
            }

            if (color.Length == 6)
            {
                color += "FF"; // add alpha if missing
            }

            var hex = Convert.ToUInt32(color, 16);
            var r = ((hex & 0xff000000) >> 0x18) / 255f;
            var g = ((hex & 0xff0000) >> 0x10) / 255f;
            var b = ((hex & 0xff00) >> 8) / 255f;
            var a = ((hex & 0xff)) / 255f;

            return new Color(r, g, b, a);
        }
    }
}