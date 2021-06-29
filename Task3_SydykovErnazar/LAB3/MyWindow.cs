using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;

namespace LAB3
{
    class MyWindow
    {
        MyGradMatrix[,] Blocks;
        public static int width = 8;
        public static int height = 16;
        public MyWindow(Point top_left, Point bottom_right, Bitmap bitmap)
        {
            int left = top_left.X;
            int top = top_left.Y;
            Blocks = new MyGradMatrix[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Blocks[i, j] = new MyGradMatrix(bitmap, i * MyGradMatrix.Size + left, j * MyGradMatrix.Size + top);
                }
            }
        }
        public double[] Properities()
        {
            double[] res = new double[width * height * MyGradMatrix.Size];

            for(int i=0;i<width;i++)
            {
                for(int j=0;j<height;j++)
                {
                    for(int k=0;k<MyGradMatrix.Size;k++)
                    {
                        res[(i *width+j) *MyGradMatrix.Size  + k] = Blocks[i, j].X[k];
                    }
                }
            }

            return res;
        }
    }
}