using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Globalization;

namespace Contal.IwQuick.Web
{
    public static class HtmlColorConvertor
    {
        public static System.Drawing.Color FromHtml(string htmlColor) 
        {
            System.Drawing.Color color;

            if (htmlColor.IndexOf("rgb") == -1)
            {
                color = System.Drawing.ColorTranslator.FromHtml(htmlColor);
            }
            else
            {
                htmlColor = htmlColor.ToLower();
                htmlColor = htmlColor.Replace(" ", "");

                bool isRgba = false;

                if (htmlColor.IndexOf("rgba") != -1)
                {
                    htmlColor = htmlColor.Replace("rgba(", "");
                    isRgba = true;
                }
                else
                    htmlColor = htmlColor.Replace("rgb(", "");
                
                htmlColor = htmlColor.Replace(")", "");
                string[] colors = htmlColor.Split(',');
                //int r = 0, g = 0, b = 0;

                if (isRgba)
                {
                    float fA = float.Parse(colors[3], CultureInfo.InvariantCulture);
                    int alpha = (int)(fA * 255);
                    color = Color.FromArgb(alpha, byte.Parse(colors[0]), byte.Parse(colors[1]), byte.Parse(colors[2]));
                }
                else
                    color = Color.FromArgb(byte.Parse(colors[0]), byte.Parse(colors[1]), byte.Parse(colors[2]));
            }

            return color;
        }
    }
}
