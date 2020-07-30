﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;
using System.Web;

namespace GDIPlusDemo
{
    public partial class SelectCourse : Form
    {
        string SID;//记录登录选课系统账户的学号
        string myCourse;
        string myCourse2;
        
        public SelectCourse(string sId)
        {
            SID = sId;
            InitializeComponent();
            toolStripStatusLabel3.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            toolStripStatusLabel1.Text = "欢迎学号为" + SID + "的同学登录选课系统！";
            timer1.Start();
            Table();
        }
        private void Table()
        {
            string sql = "select * from Course";
            Dao dao = new Dao();
            IDataReader dr = dao.Read(sql);
            while (dr.Read())
            {
                string sno, name, teacher, credit;
                sno = dr["Id"].ToString();
                name = dr["Name"].ToString();
                teacher = dr["Teacher"].ToString();
                credit = dr["Credit"].ToString();
                string[] str = { sno, name, teacher, credit };
                dataGridView1.Rows.Add(str);

            }
            dr.Close();//关闭连接
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
        }

        private void SelectCourse_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();//关闭整个程序
        }
        /// <summary>
        /// 窗体加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectCourse_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            string sql = "select * from CourseRecord where sId = '" + SID + "'";
            
            Dao dao = new Dao();
            IDataReader dr = dao.Read(sql);
            while (dr.Read())
            {
                string cId = dr["cId"].ToString();
                string sql2 = "select * from Course where Id='" + cId + "'";
                IDataReader dr2 = dao.Read(sql2);
                dr2.Read();
                string sno, name, teacher, credit;
                sno = dr2["Id"].ToString();
                name = dr2["Name"].ToString();
                teacher = dr2["Teacher"].ToString();
                credit = dr2["Credit"].ToString();
                myCourse = "学号：" + sno+"\n姓名：" + name + "\n授课教师：" + teacher + "\n学分：" + credit;
                myCourse2 = "" + sno + name + teacher + credit;
                string[] myCourse3 = { sno, name, teacher, credit };

                pictureBox1.Image = CodeImage(myCourse,myCourse2, myCourse3);
                dr2.Close();

            }
            dr.Close();//关闭连接
        }

        private void 选课ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cId = dataGridView1.SelectedCells[0].Value.ToString();//获取选中的课程号
            string sql_1 = "select *from CourseRecord where sId = '"+SID+"'and cId = '"+cId+"'";
            Dao dao = new Dao();
            IDataReader dc = dao.Read(sql_1);

            if (!dc.Read())
            {
                string sql = "insert into CourseRecord values('" + SID + "','" + cId + "')";
                int i = dao.Excute(sql);
                if (i > 0)
                {
                    MessageBox.Show("选课成功!");
                }
                this.dataGridView1.Sort(this.dataGridView1.Columns["课程编号"], ListSortDirection.Ascending);

            }
            else
            {
                MessageBox.Show("不允许重复选课！");
            }  
        }

        private void 我的课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySelect mySelect = new MySelect(SID);
            mySelect.ShowDialog();
            //this.Activate();

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dao dao = new Dao();
            dao.restart();
           
        }

        private void 修改个人密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyPassWord modifyPassWord = new ModifyPassWord(SID);
            modifyPassWord.ShowDialog();

        }

        private void 重启ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dao dao = new Dao();
            dao.restart();
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //若行已是选中状态就不再进行设置
                    if (dataGridView1.Rows[e.RowIndex].Selected == false)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dataGridView1.SelectedRows.Count == 1)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }
                    //弹出操作菜单
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }

        }

        private void 选择课程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cId = dataGridView1.SelectedCells[0].Value.ToString();//获取选中的课程号
            string sql_1 = "select *from CourseRecord where sId = '" + SID + "'and cId = '" + cId + "'";
            Dao dao = new Dao();
            IDataReader dc = dao.Read(sql_1);

            if (!dc.Read())
            {
                string sql = "insert into CourseRecord values('" + SID + "','" + cId + "')";
                int i = dao.Excute(sql);
                if (i > 0)
                {
                    MessageBox.Show("选课成功!");
                }
            }
            else
            {
                MessageBox.Show("不允许重复选课！");
            }
        }
        public Bitmap CodeImage(string str,string str2,string[] str3)
        {
            



            //动态生成html页面
            
            StringBuilder htmltext = new StringBuilder();
            try
            {
                //string ModelPath = HttpContext.Current.Server.MapPath(@"~/template.html");
                using(StreamReader sr = new StreamReader("C:\\vs_project\\Student\\template.html"))
                {
                    string line;
                    while((line = sr.ReadLine())!=null){
                        htmltext.Append(line);
                    }
                    sr.Close();
                }
            }
            catch
            {
                MessageBox.Show("文件读写错误！");
            }
            
            //MessageBox.Show(format[0]);
            for(int i = 0; i < 4; i++)
            {
                htmltext.Replace("$form["+i+"]$", str3[i]);
            }
            try
            {
                using (StreamWriter sw = new StreamWriter("E:\\html\\test.html", false, System.Text.Encoding.GetEncoding("GBK")))//保存地址
                {
                    sw.WriteLine(htmltext);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
                MessageBox.Show("该目录不能被写入！");
            }


            //拼接文件名，上传ftp服务器
            string file_path = "E:\\html\\"+"test.html";
            string ftp_path = "122.51.231.110:21";
            FtpUpLoad ftpUpLoad = new FtpUpLoad();
            ftpUpLoad.Upload(file_path, ftp_path);


            System.Drawing.Bitmap bt;
            //实例化一个生成二维码的对象
            QRCodeEncoder qrEncoder = new QRCodeEncoder();
            //设置二维码的编码模式
            qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //二维码像素宽度
            qrEncoder.QRCodeScale = 4;
            //设置版本
            qrEncoder.QRCodeVersion = 7;
            //根据内容生成二维码图像
            //Bitmap image = qrEncoder.Encode(str, Encoding.UTF8);
            string html_path = "https://caokunpeng.xyz/html/test.html";
            bt = qrEncoder.Encode(html_path, Encoding.UTF8);


            return bt;
        }
        

        private void 扫码查看ToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            
            pictureBox1.Visible = true;
            
            
            //CodeImage(course);

        }
        
        private void 扫码查看ToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
        }
        private void 我的课程ToolStripMenuItem_DoubleClick(object sender, EventArgs e)
        {
            MySelect mySelect = new MySelect(SID);
            mySelect.ShowDialog();
        }

        private void 本地查看ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySelect mySelect = new MySelect(SID);
            mySelect.ShowDialog();
        }

    }
}
