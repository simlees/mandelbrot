﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mandelbrot
{
    public partial class Form1 : Form
    {

        public struct HSBColor
        {
            float h;
            float s;
            float b;
            int a;
            public HSBColor(float h, float s, float b)
            {
                this.a = 0xff;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }
            public HSBColor(int a, float h, float s, float b)
            {
                this.a = a;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }
            public float H
            {
                get { return h; }
            }
            public float S
            {
                get { return s; }
            }
            public float B
            {
                get { return b; }
            }
            public int A
            {
                get { return a; }
            }
            public Color Color
            {
                get
                {
                    return FromHSB(this);
                }
            }
            public static Color FromHSB(HSBColor hsbColor)
            {
                float r = hsbColor.b;
                float g = hsbColor.b;
                float b = hsbColor.b;
                if (hsbColor.s != 0)
                {
                    float max = hsbColor.b;
                    float dif = hsbColor.b * hsbColor.s / 255f;
                    float min = hsbColor.b - dif;
                    float h = hsbColor.h * 360f / 255f;










                    if (h < 60f)
                    {
                        r = max;
                        g = h * dif / 60f + min;
                        b = min;
                    }
                    else if (h < 120f)
                    {
                        r = -(h - 120f) * dif / 60f + min;
                        g = max;
                        b = min;
                    }
                    else if (h < 180f)
                    {
                        r = min;
                        g = max;
                        b = (h - 120f) * dif / 60f + min;
                    }
                    else if (h < 240f)
                    {
                        r = min;
                        g = -(h - 240f) * dif / 60f + min;
                        b = max;
                    }
                    else if (h < 300f)
                    {
                        r = (h - 240f) * dif / 60f + min;
                        g = min;
                        b = max;
                    }
                    else if (h <= 360f)
                    {
                        r = max;
                        g = min;
                        b = -(h - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                }
                return Color.FromArgb
                    (
                        hsbColor.a,
                        (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(b, 0), 255))
                    );
            }
        }

        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye, hueDifference;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished, hueSliderShowing, statusBarShowing;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse down");
            xs = e.X;
            ys = e.Y;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {

            Console.WriteLine("Mouse up");
            int z, w;

            //e.consume();
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot(hueDifference);
                rectangle = false;

                Refresh();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left == MouseButtons)
            {

                Console.WriteLine("This should appear when you drag the mouse! ");


                //e.consume();
                if (action)
                {
                    xe = e.X;
                    ye = e.Y;
                    rectangle = true;
                    //repaint();
                    Refresh();
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form1().Show();
        }

        private void hueSlider_ValueChanged(object sender, EventArgs e)
        {
            hueDifference = hueSlider.Value;
            mandelbrot(hueDifference);
            Refresh();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG (*.jpg)|*.jpg";
            dialog.DefaultExt = "jpg";
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picture.Width);
                int height = Convert.ToInt32(picture.Height);
                Bitmap bitmap = new Bitmap(width, height);
                this.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));
                picture.Save(dialog.FileName, ImageFormat.Jpeg);
            }
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!statusBarShowing)
            {
                statusTextBox.Visible = true;
                statusBarShowing = true;
            }
            else
            {
                statusTextBox.Visible = false;
                statusBarShowing = false;
            }
        }

        private void hueSliderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!hueSliderShowing)
            {
                hueSlider.Visible = true;
                hueSliderShowing = true;
            }
            else
            {
                hueSlider.Visible = false;
                hueSliderShowing = false;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            x1 = Size.Width;
            y1 = Size.Height;
            // Need to do some more here

            picture = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(picture);

            mandelbrot(hueDifference);
            Refresh();
        }

        // Add change colour functionality?

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImageUnscaled(picture, 0, 0);

            if (rectangle)
            {
                Pen pen = new Pen(Color.White, 1);
                if (xs < xe)
                {
                    if (ys < ye) g.DrawRectangle(pen, xs, ys, (xe - xs), (ye - ys));
                    else g.DrawRectangle(pen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g.DrawRectangle(pen, xe, ys, (xs - xe), (ye - ys));
                    else g.DrawRectangle(pen, xe, ye, (xs - xe), (ys - ye));
                }
            }
            
        }

        private static float xy;
        private Image picture;
        private Image rectangleImage;

        private Graphics g1;
        private Cursor c1, c2;

        public Form1()
        {
            InitializeComponent();

            // Changes form control style to complete painting in buffer then output to screen
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            init();
            start();
        }

        public void init() // all instances will be prepared
        {

            finished = false;
            //c1 = new Cursor(Cursor.WAIT_CURSOR);
            //c2 = new Cursor(Cursor.CROSSHAIR_CURSOR);
            x1 = Size.Width;
            y1 = Size.Height;
            hueDifference = 0;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(picture);
            finished = true;
            statusBarShowing = true;
            hueSliderShowing = false;
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot(hueDifference);
        }

        private void mandelbrot(int hueDifference) // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            

            Pen pen = new Pen(Color.Black, 1);


            action = false;
            this.Cursor = Cursors.WaitCursor;
            statusTextBox.Text = "Mandelbrot-Set will be produced - please wait...";


            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    
                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightnes
                                          ///djm added
                                          ///

                        HSBColor hsb = new HSBColor((h * 255 + hueDifference), 0.8f * 255, b * 255);

                        Pen p = new Pen(hsb.Color, 1);

                        pen.Color = hsb.Color;

                        //pen.Color = HSBColor.FromHSB(h * 255, 0.8f * 255, b);

                        //djm 
                        alt = h;
                    }
                    g1.DrawLine(pen, x, y, x + 1, y);
                }
            statusTextBox.Text = "Mandelbrot-Set ready - please select zoom area with pressed mouse.";
            this.Cursor = Cursors.Cross;
            action = true;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }
    }
}
