using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mul4Image
{
    public partial class Form1 : Form
    {
        BuildMul4Texture build = new BuildMul4Texture();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = "处理中....";
            var inFile = new FileInfo(textBox1.Text);
            var parent = inFile.Directory;
            build.RebuildMul4Png = copyNot4Mul.Checked;
            build.CopyNotPng = copyNotPng.Checked;
            if (build.IsDir(inFile))
            {
                var inDirFile = new DirectoryInfo(textBox1.Text);
                build.Process(inDirFile, new DirectoryInfo(parent.FullName + "/" + inDirFile.Name + "_Resize"));
            }
            else
            {
                var grandParent = inFile.Directory.Parent;
                build.Convert(inFile, build.CreateOutFile(
                    parent,
                    new DirectoryInfo(grandParent.FullName + "/" + parent.Name + "_Resize"), 
                    inFile));
            }
            label2.Text = "已完成";
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            Array file = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            foreach(var f in file)
            {
                string str = f.ToString();

                var info = new FileInfo(str);
                label2.Text = "已获得文件";
                textBox1.Text = info.FullName;
                return;
            }
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))    //判断拖来的是否是文件  
                e.Effect = DragDropEffects.Link;                //是则将拖动源中的数据连接到控件  
            else e.Effect = DragDropEffects.None;
        }
    }
}
