﻿using System;
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
        public static string pipeiFilepath;//静态变量便于画板传回路径
        private static int FinallResult;
        private double N = 0;//数据文件存储的图片总数
        private int[] Ni = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//数据文件里面的每个数字的图片个数
        private int[,] tongji;//每种数字的28*28特征值得计数
        private double[,] PjWi ;//每个特征点的比例
        private double[] PWi = new double[10];//模板库里面每个数字的比例
        private bool cal=false;//是否计算了先验和类条件


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
            cal = false;//模板库改变 需要重新计算先验  类条件概率
            if (textBox2.Text.Length < 1)

            {
                MessageBox.Show("请输入准备导入的图片的正确数字");
                return;
            }
            if (width.Text.Length < 1 || height.Text.Length < 1)
            {
                MessageBox.Show("请输入准备切割图片大小！");
                return;
            }

            Bitmap thePic = new Bitmap(Path.Text);
            String data;
            data = picCut(thePic, pictureBox1, pictureBox3);
            data = textBox2.Text + data;
            MessageBox.Show(data + "----------" + data.Length.ToString());
            FileStream fs = new FileStream("data.txt", FileMode.Append);
            StreamWriter writeStream = new StreamWriter(fs);
            writeStream.WriteLine(data + "\n");
            writeStream.Flush();
            fs.Flush();
            fs.Close();
            MessageBox.Show("学习成功！");




        }
        private void button3_Click(object sender, EventArgs e)
        {
         
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//支持多选
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
                Form1.pipeiFilepath = dialog.FileName;
            }
        }

        public void button4_Click(object sender, EventArgs e)
        {

            switch (listBox1.SelectedIndex)
            {
                case -1:
                    MessageBox.Show("请选择匹配方式！");
                    break;
                case 0:
                    ouShiJuLiPiPei();
                    break;
                case 1:
                    bysErZhiShuJu();
                    break;

                default: return;
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {
            cal = false;
            for (int i = 0; i < 1000; i++)
            {
                Bitmap thePic_0 = new Bitmap(@".\train\" + i / 100 + "_" + (i % 100).ToString() + ".bmp");
                Bitmap thePic = thePic_0.Clone(new Rectangle(0, 0, thePic_0.Width, thePic_0.Height), PixelFormat.Format24bppRgb);
                String data;
                data = picCut(thePic, pictureBox1, pictureBox3);
                data = (i / 100).ToString().ToCharArray()[0] + data;
                // MessageBox.Show(data + "----------" + data.Length.ToString());

                FileStream fs = new FileStream("data.txt", FileMode.Append);
                StreamWriter writeStream = new StreamWriter(fs);
                writeStream.WriteLine(data + "\n");
                writeStream.Flush();
                fs.Flush();
                fs.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            (new draw()).ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button7.Enabled = false;
            button8.Enabled = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Length < 1)
            {
                MessageBox.Show("请输入正确的数字");
                return;
            }
            Bitmap thePic = new Bitmap(pipeiFilepath);
            String data;
            data = picCut(thePic, pictureBox1, pictureBox3);
            data = textBox3.Text + data;
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
            MessageBox.Show("学习成功！");
            FileStream fss = new FileStream("data.log", FileMode.Append);
            StreamWriter writeStreamm = new StreamWriter(fss);
            //时间 错误 实际 本应该是
            writeStreamm.WriteLine(DateTime.Now.ToString("yyyyMMddhhmmss") + " " + "" + FinallResult.ToString() + "" + "" + textBox3.Text + "" + "错误" + "\n");
            writeStreamm.Flush();
            fss.Flush();
            fss.Close();
            button7.Enabled = false;
            button8.Enabled = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {

            FileStream fs = new FileStream("data.log", FileMode.Append);
            StreamWriter writeStream = new StreamWriter(fs);
            writeStream.WriteLine(DateTime.Now.ToString("yyyyMMddhhmmss") + " " + "" + FinallResult.ToString() + "" + "" + FinallResult.ToString() + "" + "正确" + "\n");
            writeStream.Flush();
            fs.Flush();
            fs.Close();
            button7.Enabled = false;
            button8.Enabled = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int all = 0;
            int rightall = 0;
            StreamReader read = File.OpenText(@".\data.log");
            int[][] result = new int[2][];
            result[0] = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            result[1] = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            string a;
            while ((a = read.ReadLine()) != null)
            {
                read.ReadLine();//吃掉\n
                all++;
                result[1][a.ToCharArray()[16] - '0']++;
                //
                if (a.Substring(17) == "正确")
                {
                    //MessageBox.Show(a.Substring(17));
                    rightall++;
                }
                if (a.ToCharArray()[16] == a.ToCharArray()[15])
                {
                    result[0][a.ToCharArray()[16] - '0']++;
                }


            }
            if (all != 0)
            {
                richTextBox1.AppendText("总识别率为   " + (((double)rightall) / ((double)all) * 100.0).ToString() + "%\n");
            }
            else
                richTextBox1.AppendText("没有历史数据！\n");
            for (int c = 0; c < 10; c++)
            {
                if (result[1][c] != 0)
                    richTextBox1.AppendText("" + c + "的识别率为   " + (((double)result[0][c] / (double)result[1][c]) * 100).ToString() + '%' + '\n');
                else
                    richTextBox1.AppendText("" + c + "没有数据" + '\n');
            }

        }
        private String picCut(Bitmap thePic, PictureBox aa, PictureBox bb)//切割图片以及显示方块和二值图片 返回01数据串
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
                    data = data + "" + turnPic(heng, zong, thePic.Width / int.Parse(width.Text), thePic.Height / int.Parse(height.Text), blackWhite).ToString() + "";
                }


            Bitmap newPic = new Bitmap(200, 200);
            for (int zong = 0; zong < int.Parse(height.Text); zong++)
                for (int heng = 0; heng < int.Parse(width.Text); heng++)
                {
                    if ((data.ToCharArray()[zong * int.Parse(width.Text) + heng] - '0') > 0)
                        setPic(heng, zong, 200 / int.Parse(width.Text), 200 / int.Parse(height.Text), newPic, Color.Black);
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
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);


                    newImg.SetPixel(i, j, newColor);
                }
            }

            return newImg;
        }
        private int turnPic(int heng, int zong, int eachWidth, int eachHeight, Bitmap blackWhite)//返回数据01 单个数据
        {
            //  MessageBox.Show(blackWhite.Height.ToString()+'-'+ eachHeight.ToString());
            for (int eachHei = 0; eachHei < eachHeight; eachHei++)
                for (int eachWid = 0; eachWid < eachWidth; eachWid++)
                {
                    if (int.Parse(blackWhite.GetPixel(eachWidth * heng + eachWid, eachHeight * zong + eachHei).B.ToString()) != 0)
                    {
                        return 0;//白色的返回0
                    }

                }
            return 1;//黑色的返回1
        }
        private void setPic(int heng, int zong, int eachWidth, int eachHeight, Bitmap blackWhite, Color color)//给每一块赋值 要么黑要么白  0-1
        {
            for (int a = 0; a < eachHeight; a++)
                for (int b = 0; b < eachWidth; b++)
                {
                    // if (int.Parse(blackWhite.GetPixel().B.ToString()) != 0)
                    blackWhite.SetPixel(eachWidth * heng + b, eachHeight * zong + a, color);
                }

        }

        

        private void bys()//计算先验  类条件概率
        {
            this.tongji = new int[10, int.Parse(width.Text) * int.Parse(height.Text)];
            this.PjWi = new double[10, int.Parse(width.Text) * int.Parse(height.Text)];
            //计算先验概率

            for (int x = 0; x < 10; x++)
                for (int y = 0; y < int.Parse(width.Text) * int.Parse(height.Text); y++)
                    PjWi[x, y] = 0;

            StreamReader read = File.OpenText(@".\data.txt");
            string a;
            while ((a = read.ReadLine()) != null)
            {

                read.ReadLine();//吃掉\n
                N++;
                Ni[a.ToCharArray()[0] - '0']++;
                for (int i = 0; i < int.Parse(width.Text) * int.Parse(height.Text); i++)
                {
                    if (a.ToCharArray()[i + 1] == '0')
                        PjWi[a.ToCharArray()[0] - '0', i]++;

                }
            }
            read.Close();
            for (int i = 0; i < 10; i++)//先验概率
                PWi[i] = Ni[i] / N;
            //计算类条件概率
            richTextBox1.Clear();            
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < int.Parse(width.Text) * int.Parse(height.Text); y++)
                {
                    PjWi[x, y] = (PjWi[x, y] + 1.0) / (Ni[x] + 2.0);//特征值的比例
                }               
            }
            cal = true;//告诉程序已经计算过一次
        }
   private void bysErZhiShuJu()
        {
            try
            {
                richTextBox1.Clear();
                if(!cal)
                   bys();
 
                if (width.Text.Length < 1 || height.Text.Length < 1)
                {
                    MessageBox.Show("请输入准备切割图片大小！");
                    return;
                }
                richTextBox1.AppendText("先验概率：\n");
                for (int i = 0; i < 10; i++)//输出先验概率
                    richTextBox1.AppendText(i.ToString()+'-'+PWi[i].ToString()+'\n');
      
               
                double[] PXWi = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                
                Bitmap thePic_0 = new Bitmap(Form1.pipeiFilepath);
                Bitmap headPic = thePic_0.Clone(new Rectangle(0, 0, thePic_0.Width, thePic_0.Height), PixelFormat.Format24bppRgb);
                String data;
                data = picCut(headPic, pictureBox2, pictureBox4);
                richTextBox1.AppendText("条件概率：\n");
                //计算条件概率 
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < int.Parse(width.Text) * int.Parse(height.Text); i++)
                    {
                        if (data.ToCharArray()[i] == '0')
                            PXWi[j] = PXWi[j] * PjWi[j, i];//为0
                        else
                            PXWi[j] = PXWi[j] * (1 - PjWi[j, i]);//为1
                    }
                    richTextBox1.AppendText(j.ToString() + '-' + PXWi[j].ToString() + '\n');

                }
                //计算后验概率
                double sum = 0;
                for (int i = 0; i < 10; i++)
                {
                    sum = sum + PXWi[i] * PWi[i];
                }
                double[] result = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                richTextBox1.AppendText("后验概率:\n");
                for (int i = 0; i < 10; i++)
                {
                    result[i] = PWi[i] * PXWi[i] / sum;
                    richTextBox1.AppendText(i.ToString() + '-' + result[i].ToString() + '\n');
                }
                int finall = 0;
                double temp = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (result[i] > temp)
                    {
                        temp = result[i];
                        finall = i;
                    }
                }
                richTextBox1.AppendText("匹配的结果为："+finall.ToString() + '\n');
                FinallResult = finall;
                button8.Enabled = true;
                button7.Enabled = true;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }

        }

        private void ouShiJuLiPiPei()
        {
            try
            {
                if (width.Text.Length < 1 || height.Text.Length < 1)
                {
                    MessageBox.Show("请输入准备切割图片大小！");
                    return;
                }
                richTextBox1.Clear();
                Bitmap thePic_0 = new Bitmap(Form1.pipeiFilepath);
                Bitmap headPic = thePic_0.Clone(new Rectangle(0, 0, thePic_0.Width, thePic_0.Height), PixelFormat.Format24bppRgb);
                String data;
                data = picCut(headPic, pictureBox2, pictureBox4);


                int[] result = new int[10];
                for (int x = 0; x < 10; x++)
                    result[x] = int.Parse(width.Text) * int.Parse(height.Text);
                StreamReader read = File.OpenText(@".\data.txt");
                string a;
                while ((a = read.ReadLine()) != null)
                {
                    read.ReadLine();//吃掉\n

                    string per = piPei(a, data).ToString();

                    if (int.Parse(per) < result[a.ToCharArray()[0] - '0'])
                        result[a.ToCharArray()[0] - '0'] = int.Parse(per);


                }
                int finallRes = result[0];
                int num = 0;
                richTextBox1.AppendText("对应数字--------欧氏距离\n");
                for (int x = 0; x < 10; x++)
                {

                    richTextBox1.AppendText(x.ToString() + "----------" + (result[x]).ToString() + '\n');
                    if (result[x] < finallRes)
                    {
                        finallRes = result[x];
                        num = x;
                    }
                }
                richTextBox1.AppendText("--------------\n" + "最后匹配的结果为" + num.ToString() + '\n');


                read.Close();
                FinallResult = num;
                button8.Enabled = true;
                button7.Enabled = true;

            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);

            }

        }

        private void bysZuiXiaoCuoWu()
        {
            
        }

        private void bysZuiXiaoFengXian()
        {
           
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

      
       

    }
}
