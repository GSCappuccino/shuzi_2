using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace shuzi
{
    public partial class draw : Form
    {
        Graphics g;
        private Point p1, p2;
        private  bool drawing = false;
        Bitmap pic ;
        
        public draw()
        {
            
            InitializeComponent();      
            pic = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            for (int a = 0; a < pictureBox1.Height; a++)
                for (int b = 0; b<pictureBox1.Width; b++)
                {
                    pic.SetPixel(a,b,Color.Black);
                }
            pictureBox1.Image = pic;            
            g = Graphics.FromImage(pictureBox1.Image);
          
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
                {
            
                    p1 = new Point(e.X,e.Y);
                    p2 = new Point(e.X,e.Y);
                    drawing = true;
                }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
           drawing = false;
        }

       

        private void draw_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (drawing)
                {
                    Point currentPoint = new Point(e.X, e.Y);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawLine(new Pen(Color.White, 5), p2, currentPoint);
                    p2.X = currentPoint.X;
                    p2.Y = currentPoint.Y;

                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
           // button1.Text = DateTime.Now.ToString("yyyyMMddhhmmss");
            
            pictureBox1.DrawToBitmap(pic, new Rectangle(0, 0, pic.Width, pic.Height));
            Form1.pipeiFilepath = @"./handdraw/" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".bmp";
            pic.Save(Form1.pipeiFilepath);
            
           
            
            this.Close();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Refresh();

        }

        private void draw_Load(object sender, EventArgs e)
        {
           
           
        }
    }
}
