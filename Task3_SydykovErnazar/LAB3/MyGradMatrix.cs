using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB3
{
    class MyGradMatrix
    {
        public static int Size = 8;
        public double[,] I { get; private set; }
        public double[] IX { get; private set; }
        public double[] IY { get; private set; }
        public double[] X { get; private set; }
        public double[,] Atan { get; private set; }

        public MyGradMatrix(Bitmap bitmap, int left, int top)
        {
            I = new double[Size, Size];
            IX = new double[Size];
            IY = new double[Size];
            X = new double[Size];
            Atan = new double[Size, Size];
            if (left + Size >= bitmap.Width) left = bitmap.Width - Size;
            for (int i = left; i < left + Size; i++)
            {
                for (int j = top; j < top + Size; j++)
                {
                    Color c;
                    if (j < 1) c = Color.Black;
                    else c = bitmap.GetPixel(i, j - 1);
                    Color c2;
                    if (j >= bitmap.Height - 1) c2 = Color.Black;
                    else c2 = bitmap.GetPixel(i, j + 1);

                    double Yi1 = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    double Yi2 = 0.299 * c2.R + 0.587 * c2.G + 0.114 * c2.B;
                    double Ix = (Yi1 - Yi2);

                    if (i >= bitmap.Width - 1)  c = Color.Black;
                    else c = bitmap.GetPixel(i + 1, j);

                    if (i >= 1) c2 = bitmap.GetPixel(i - 1, j);
                    else c2 = Color.Black;

                    Yi1 = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                    Yi2 = 0.299 * c2.R + 0.587 * c2.G + 0.114 * c2.B;
                    double Iy = Yi1 - Yi2;
                    I[i - left, j - top] = Math.Sqrt(Ix * Ix + Iy * Iy);
                    var Angle = Math.Atan(Iy / Ix);

                    Atan[i - left, j - top] = Math.Atan(Iy / Ix);
                    
                    if(Angle>0&&Ix<0)
                    {
                        Angle += Math.PI;
                    }
                    if(Angle<0&&Iy<0)
                    {
                        Angle += Math.PI;
                    }
                    Angle += Math.PI / 2;
                    //for (int k = 0; k < Size; k++) 
                    //    if (Angle>(k*Math.PI*2/Size)&&
                    //        Angle< ((k+1) * Math.PI * 2 / Size)) 
                    //    { 
                    //        IX[k] += Ix; IY[k] += Iy;
                    //    };

                    if      (Angle < 8 * Math.PI / 4 && Angle > 7 * Math.PI / 4) { X[7] += I[i-left,j-top]; IX[7] += Ix; IY[7] += Iy; }
                    else if (Angle < 7 * Math.PI / 4 && Angle > 6 * Math.PI / 4) { X[6] += I[i-left,j-top]; IX[6] += Ix; IY[6] += Iy; }
                    else if (Angle < 6 * Math.PI / 4 && Angle > 5 * Math.PI / 4) { X[5] += I[i-left,j-top]; IX[5] += Ix; IY[5] += Iy; }
                    else if (Angle < 5 * Math.PI / 4 && Angle > 4 * Math.PI / 4) { X[4] += I[i-left,j-top]; IX[4] += Ix; IY[4] += Iy; }
                    else if (Angle < 4 * Math.PI / 4 && Angle > 3 * Math.PI / 4) { X[3] += I[i-left,j-top]; IX[3] += Ix; IY[3] += Iy; }
                    else if (Angle < 3 * Math.PI / 4 && Angle > 2 * Math.PI / 4) { X[2] += I[i-left,j-top]; IX[2] += Ix; IY[2] += Iy; }
                    else if (Angle < 2 * Math.PI / 4 && Angle > 1 * Math.PI / 4) { X[1] += I[i-left,j-top]; IX[1] += Ix; IY[1] += Iy; }
                    else if (Angle < 1 * Math.PI / 4 && Angle > 0 * Math.PI / 4) { X[0] += I[i-left,j-top]; IX[0] += Ix; IY[0] += Iy; }
                }
            }
            for(int i=0;i<Size;i++)
            {
                X[i] = Math.Sqrt(IX[i] * IX[i] + IY[i] * IY[i]);
            }
        }
    }
}
