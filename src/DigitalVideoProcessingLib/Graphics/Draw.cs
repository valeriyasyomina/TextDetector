using DigitalImageProcessingLib.RegionData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalVideoProcessingLib.Graphics
{
    public class Draw
    {
        public static void DrawTextBoundingBoxes(Bitmap bitmapFrame, List<TextRegion> textRegions, System.Drawing.Pen pen)
        {
            try
            {
                if (bitmapFrame == null)
                    throw new ArgumentNullException("Null bitmap frame in DrawTextBoundingBoxes");
                if (textRegions == null)
                    throw new ArgumentNullException("Null textRegions in DrawTextBoundingBoxes");
                if (pen == null)
                    throw new ArgumentNullException("Null pen in DrawTextBoundingBoxes");

                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmapFrame);

                if (textRegions != null)
                {
                    for (int i = 0; i < textRegions.Count; i++)
                        graphics.DrawRectangle(pen, textRegions[i].MinBorderIndexJ, textRegions[i].MinBorderIndexI,
                            textRegions[i].MaxBorderIndexJ - textRegions[i].MinBorderIndexJ, textRegions[i].MaxBorderIndexI - textRegions[i].MinBorderIndexI);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
