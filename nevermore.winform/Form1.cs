using AxWpsDocFrame;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nevermore.winform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AxKDocFrame doc1 = new AxKDocFrame();
            AxKDocFrame doc2 = new AxKDocFrame();
            doc2.Height = 400;
            doc2.Width = 400;
            ((System.ComponentModel.ISupportInitialize)(doc1)).BeginInit();
            this.tabPage1.Controls.Add(doc1);
            ((System.ComponentModel.ISupportInitialize)(doc1)).EndInit();

            ((System.ComponentModel.ISupportInitialize)(doc2)).BeginInit();
            this.tabPage2.Controls.Add(doc2);
            ((System.ComponentModel.ISupportInitialize)(doc2)).EndInit();
            doc1.Open(@"E:\DownLoad\孙建远\互联网调解系统V3.7.1.3-需求规格说明书-0421.docx");
            doc2.Open(@"E:\邮箱附件\附件四：OA新员工培养流程填写说明.docx");

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
