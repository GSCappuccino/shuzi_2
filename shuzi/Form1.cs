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
    public partial class Form1 : Form
    {
        public static Bitmap drawmap;
        public Form1()
        {
            InitializeComponent();
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String FilePath;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//支持多选
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Path.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
                
                Bitmap thePic = new Bitmap(Path.Text);
                String data;
                data = picCut(thePic,pictureBox1,pictureBox3);
                data = textBox2.Text + data;
                MessageBox.Show(data + "----------" + data.Length.ToString());
                if (File.Exists("data.txt") == false)
                {
                    File.Create("data.txt");
                }
                FileStream fs = new FileStream("data.txt", FileMode.Append);
                StreamWriter writeStream = new StreamWriter(fs);
                writeStream.WriteLine(data + "\n");
                writeStream.Flush();
                fs.Flush();
                fs.Close();
                MessageBox.Show("导入成功！");
          
            
           

        }
        private String picCut(Bitmap thePic, PictureBox aa, PictureBox bb)//切割图片以及显示方块和二值图片
        {
            String data = null;
           
            // MessageBox.Show(thePic.GetPixel(1,1).B.ToString());
            Bitmap blackWhite = ConvertTo1Bpp(thePic);
            aa.Width = blackWhite.Width;
            aa.Height = blackWhite.Height;
            aa.Image = blackWhite;//显示图像

            

            for (int zong = 0; zong < int.Parse(height.Text); zong++)
                for (int heng = 0; heng < int.Parse(width.Text); heng++)
                {
                    data = data + "" + turnPic(heng, zong,thePic.Width / int.Parse(width.Text) ,thePic.Height /int.Parse( height.Text) , blackWhite).ToString() + "";
                }


            Bitmap newPic = new Bitmap(200, 200);
            for (int zong = 0; zong < int.Parse(height.Text); zong++)
                for (int heng = 0; heng < int.Parse(width.Text); heng++)
                {
                    if((data.ToCharArray()[zong* int.Parse(width.Text) + heng]-'0')>0)
                        setPic(heng, zong, 200/ int.Parse(width.Text),200/ int.Parse(height.Text) , newPic,Color.Black);
                    else
                        setPic(heng, zong, 200 / int.Parse(width.Text), 200 / int.Parse(height.Text), newPic, Color.White);
                    bb.Image = newPic;
                  
                }


            
                    return data;
                
        }
        public static Bitmap ConvertTo1Bpp(Bitmap bmp)//二值化
        {
            Bitmap newImg = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255,255, 255);
                   
                    
                    newImg.SetPixel(i, j, newColor);
                }
            }
       
            return newImg;
        }
        private int turnPic(int heng,int zong,int eachWidth,int eachHeight,Bitmap blackWhite)//返回数据01
        {
          //  MessageBox.Show(blackWhite.Height.ToString()+'-'+ eachHeight.ToString());
            for (int eachHei = 0; eachHei < eachHeight; eachHei++)
                for (int eachWid = 0; eachWid < eachWidth; eachWid++)
                {
                    if (int.Parse(blackWhite.GetPixel(eachWidth * heng + eachWid, eachHeight * zong + eachHei).B.ToString()) != 0)
                    {
                        return 0;
                    }

                }
            return 1;
        }
        private void setPic(int heng, int zong, int eachWidth, int eachHeight, Bitmap blackWhite,Color color)//给一张图片赋值
        {
            for (int a = 0; a < eachHeight; a++)
                for (int b = 0; b < eachWidth; b++)
                {
                    // if (int.Parse(blackWhite.GetPixel().B.ToString()) != 0)
                    blackWhite.SetPixel(eachWidth * heng + b, eachHeight * zong + a,color);
                }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String FilePath;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//支持多选
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Clear();
                Bitmap thePic_0 = new Bitmap(textBox1.Text);
                Bitmap headPic = thePic_0.Clone(new Rectangle(0, 0, thePic_0.Width, thePic_0.Height), PixelFormat.Format24bppRgb);
                String data;
                data = picCut(headPic, pictureBox2,pictureBox4);

                StreamReader read = File.OpenText(@".\data.txt");
                string a;
                int[] result = new int[10];
                for (int x = 0; x < 10; x++)
                    result[x] = int.Parse(width.Text) * int.Parse(height.Text);

                while ((a = read.ReadLine()) != null)
                {
                    read.ReadLine();//吃掉\n

                    string per = piPei(a, data).ToString();
                  
                    if (int.Parse(per) < result[a.ToCharArray()[0] - '0'])
                        result[a.ToCharArray()[0] - '0'] = int.Parse(per);


                }
                int finallRes = result[0];
                int num = 0;
                for (int x = 0; x < 10; x++)
                {

                    richTextBox1.AppendText((result[x]).ToString() + "----" + x.ToString() + '\n');
                    if (result[x] < finallRes)
                    {
                        finallRes = result[x];
                        num = x;
                    }
                }
                richTextBox1.AppendText("--------------\n" + "最后匹配的结果为" + num.ToString() + '\n');


                read.Close();
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
           
            for (int i = 0; i < 1000; i++)
            {                            
                Bitmap thePic_0 = new Bitmap(@".\train\"+i/100+"_"+(i%100).ToString()+".bmp");
                Bitmap thePic = thePic_0.Clone(new Rectangle(0, 0, thePic_0.Width, thePic_0.Height), PixelFormat.Format24bppRgb);
                String data;
                data = picCut(thePic,pictureBox1,pictureBox3);
                data = (i/100).ToString().ToCharArray()[0] + data;
               // MessageBox.Show(data + "----------" + data.Length.ToString());
               
                FileStream fs = new FileStream("data.txt", FileMode.Append);
                StreamWriter writeStream = new StreamWriter(fs);
                writeStream.WriteLine(data + "\n");
                writeStream.Flush();
                fs.Flush();
                fs.Close();
            }
        }

        private int piPei(String model,String data)
        {
            int per=0;
            for (int i=0;i<data.Length;i++)
            {
                per = per + (model.ToCharArray()[i+1] - data.ToCharArray()[i]) * (model.ToCharArray()[i + 1] - data.ToCharArray()[i]);
            }
            return per;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            (new draw()).ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
