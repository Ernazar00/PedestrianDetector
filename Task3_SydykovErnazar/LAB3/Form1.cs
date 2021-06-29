using System;
using System.IO;
using Accord.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;

namespace LAB3
{
    public partial class Form1 : Form
    {
        SupportVectorMachine MySVM;
        List<string> TrainingCoordinatesList = new List<string>();
        struct Rect
        {
            public int x1, x2;
            public int y1, y2;
            public override string ToString()
            {
                return "x1=" + x1 + " y1=" + y1;
            }
        }
        List<Rect> TrainingWindows = new List<Rect>();
        List<Rect> TestingWindows = new List<Rect>();
        MyWindow[] windows;
        delegate void DoAny();

        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                try 
                {
                    filePathBox.Text = openFileDialog1.FileName;
                    string dir = openFileDialog1.FileName;

                    FileStream file = new FileStream(dir, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(file);
                    string line = reader.ReadLine();

                    richTextBox1.AppendText(dir  + "\n");
                    while (line != null)
                    {
                        TrainingCoordinatesList.Add(line);
                        string[] Coord = line.Split('\t');
                        Rect rect;
                        rect.x1 = int.Parse(Coord[2]);
                        rect.x2 = int.Parse(Coord[4]);
                        rect.y1 = int.Parse(Coord[1]);
                        rect.y2 = int.Parse(Coord[3]);
                        TrainingWindows.Add(rect);

                        richTextBox1.AppendText(line + "\n");
                        Application.DoEvents();
                        line = reader.ReadLine();
                    }
                    reader.Close(); file.Close();
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string dir = Directory.GetParent(filePathBox.Text).FullName;
                    Invoke(new DoAny(() =>
                    {
                        progressBar1.Maximum = TrainingWindows.Count*2;
                    }));
                    windows = new MyWindow[TrainingWindows.Count*2];
                    Random random = new Random(DateTime.Now.Millisecond);
                    double[][] X = new double[windows.Length][];
                    int[] y = new int[windows.Length];

                    for (int image = 0; image < windows.Length; image++)
                    {
                        string img = dir + "\\" + (image+1) + ".png";
                        if(image>=windows.Length/2) 
                            img = dir + "\\" + (image-windows.Length/2 + 1) + ".png";

                        if (File.Exists(img))
                        {
                            Bitmap bit = new Bitmap(img);
                            try
                            {
                                if(image >= windows.Length/2)
                                {
                                    int i = image - windows.Length / 2;
                                    Point top_left = new Point(TrainingWindows[i].x1, TrainingWindows[i].y1);
                                    Point bottom_right = new Point(TrainingWindows[i].x2, TrainingWindows[i].y2);

                                    windows[image] = new MyWindow(top_left, bottom_right, bit);
                                    X[image] = windows[i].Properities();
                                    y[image] = 1; 
                                    
                                    Invoke(new DoAny(() =>
                                    {
                                        progressBar1.Value = image;
                                        Bitmap bit2 = (Bitmap)bit.Clone();
                                        for (int j = top_left.Y; j < bottom_right.Y; j++) bit2.SetPixel(top_left.X, j, Color.Green);   
                                            for (int j = top_left.Y; j < bottom_right.Y; j++) bit2.SetPixel(bottom_right.X, j, Color.Green);   
                                        
                                        pictureBox1.Image = bit2;
                                        Application.DoEvents();
                                        Invalidate();
                                        Refresh();
                                    }));
                                }
                                else
                                {
                                    int x = image;
                                    Point top_left = new Point(TrainingWindows[x].x1, TrainingWindows[x].y1);
                                    Point bottom_right = new Point(TrainingWindows[x].x2, TrainingWindows[x].y2);
                                    int dx = 0; 
                                    if(top_left.X+80< bit.Width / 2)
                                    {
                                        dx = random.Next() % (bit.Width/2 - 85)+ bit.Width / 2;
                                    }
                                    else if(top_left.X> bit.Width / 2)
                                    {
                                        dx = random.Next() % (bit.Width/2 - 85);
                                    }
                                    else
                                    {
                                        dx = 0;
                                    }
                                    Point top_left2 = new Point(dx, 0);
                                    Point bottom_right2 = new Point(dx + 80, 200);
                                    windows[image] = new MyWindow(top_left2, bottom_right2, bit);

                                    X[image] = windows[image].Properities();
                                    y[image] = -1;

                                    Invoke(new DoAny(() =>
                                    {
                                        progressBar1.Value = image;

                                        Bitmap bit2 = (Bitmap)bit.Clone();
                                        for (int j = top_left.Y; j < bottom_right.Y; j++) bit2.SetPixel(top_left2.X, j, Color.Red);
                                        for (int j = top_left.Y; j < bottom_right.Y; j++) 
                                            bit2.SetPixel(bottom_right2.X, j, Color.Red);

                                        pictureBox1.Image = bit2;
                                        Application.DoEvents();
                                        Invalidate();
                                        Refresh();
                                    }));
                                }
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.Message+ex.StackTrace+"\n"+image);
                            }
                        }
                    }
                    var teach = new LinearDualCoordinateDescent();
                    teach.Loss = Loss.L2;
                    teach.Complexity = 1000;
                    teach.Tolerance = 1e-5;

                    MySVM = teach.Learn(X, y);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+ex.StackTrace);
                }
            });
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
            {     
                try
                {
                        
                    Bitmap bit = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = bit;
                    Invoke(new DoAny(() =>
                    {
                        progressBar1.Maximum = bit.Width - MyGradMatrix.Size * MyWindow.width;
                    }));
                    for (int i=0;i<bit.Width-MyGradMatrix.Size*MyWindow.width;i++)
                    {
                        try
                        {
                            Point top_left = new Point(i, 0);
                            Point bottom_right = new Point(i+ MyGradMatrix.Size * MyWindow.width, bit.Height);

                            var wind = new MyWindow(top_left, bottom_right, bit);
                            var X = wind.Properities();

                            bool preds = MySVM.Decide(X);
                            double score = MySVM.Score(X);
                            Invoke(new DoAny(() =>
                            {
                                progressBar1.Value = i;

                                Bitmap bit2 = (Bitmap)bit.Clone();
                                for (int j = top_left.Y; j < bottom_right.Y; j++) bit2.SetPixel(top_left.X, j, Color.Yellow);
                                    for (int j = top_left.Y; j < bottom_right.Y; j++) bit2.SetPixel(bottom_right.X, j, Color.Yellow);
                                richTextBox1.Text += i + " " + preds + " " + score + "\n";
                                if (preds)
                                {
                                    for (int j = top_left.Y; j < bottom_right.Y; j++) bit.SetPixel(top_left.X, j, Color.Green);
                                    for (int j = top_left.Y; j < bottom_right.Y; j++) bit.SetPixel(bottom_right.X, j, Color.Green);
                                }
                                pictureBox1.Image = bit2;
                                Application.DoEvents();
                                Invalidate();
                                Refresh();
                            }));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + ex.StackTrace);
                        }
                    }                     
                        
                }            
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }
        private void readSvmBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if(openFileDialog1.ShowDialog()==DialogResult.OK)
                {
                    var model = LibSvmModel.Load(openFileDialog1.FileName);
                    MySVM = model.CreateMachine();

                    var file = File.ReadAllText(openFileDialog1.FileName);
                    richTextBox1.AppendText(file);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if(MySVM!=null)
            {
                if(saveFileDialog1.ShowDialog()==DialogResult.OK)
                {
                    var model = LibSvmModel.FromMachine(MySVM);
                    model.Save(saveFileDialog1.FileName);
                    richTextBox1.AppendText("Сохранено"+ saveFileDialog1.FileName+"\n");
                }
            }
        }
    }
}