using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mul4Image
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class BuildMul4Texture
    {
        bool copyMul4Png = true;

        bool copyNotPng;

        public bool RebuildMul4Png { get => copyMul4Png; set => copyMul4Png = value; }
        public bool CopyNotPng { get => copyNotPng; set => copyNotPng = value; }

        public void RebuildPng(FileInfo src, FileInfo dst)
        {
            var bitmapIn = new Bitmap(src.FullName);
            bitmapIn.Save(dst.FullName, ImageFormat.Png);
            bitmapIn.Dispose();
        }
        public void CopyFile(FileInfo src, FileInfo dst)
        {
            src.CopyTo(dst.FullName);
            //var read = src.OpenRead();
            //var write = dst.OpenWrite();
            //byte[] buf = new byte[1024*512];
            //var readCount = read.Read(buf,0,buf.Length);
            //while(readCount > 0)
            //{
            //    write.Write(buf, 0, readCount);
            //    readCount = read.Read(buf, 0, buf.Length);
            //}
            //read.Close();
            //read.Dispose();
            //write.Close();
            //write.Dispose();
        }
        public void Printf(string log)
        {
        }
        public void Process(string inRoot, string outRoot)
        {
            Process(new DirectoryInfo(inRoot), new DirectoryInfo(outRoot));
        }
        public void CollectFile(DirectoryInfo root, List<FileInfo> pngs, IsCollect IsCollect)
        {
            var files = root.GetFiles();
            foreach (var file in files)
            {
                if (IsCollect(file))
                {
                    pngs.Add(file);
                }
            }
            var dirs = root.GetDirectories();
            foreach (var dir in dirs)
            {
                CollectFile(dir, pngs, IsCollect);
            }
        }
        public delegate bool IsCollect(FileInfo file);
        public bool IsImageCollect(FileInfo file)
        {
            return true;
        }
        public void Process(DirectoryInfo inDir, DirectoryInfo outDir)
        {
            List<FileInfo> pngs = new List<FileInfo>();
            CollectFile(inDir, pngs, IsImageCollect);
            foreach (var file in pngs)
            {
                Convert(file, CreateOutFile(inDir, outDir, file));
            }
        }
        public FileInfo CreateOutFile(DirectoryInfo inRoot, DirectoryInfo outRoot, FileInfo file)
        {
            var outFile = new FileInfo(CreateCombine(FormatPath(inRoot), FormatPath(outRoot), file));
            CreateDir(outFile);
            return outFile;
        }
        public FileInfo CreateOutFile(string inRoot, string outRoot, FileInfo file)
        {
            var outFile = new FileInfo(CreateCombine(inRoot, outRoot, file));
            CreateDir(outFile);
            return outFile;
        }
        public void CreateDir(FileInfo file)
        {
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
        }
        public string CreateCombine(string inRoot, string outRoot, FileInfo file)
        {
            return FormatPath(file).Replace(inRoot, outRoot);
        }
        public void Convert(FileInfo inFile, FileInfo outFile)
        {
            if (IsDir(inFile))
            {
                Printf("is dir:" + inFile.FullName);
                return;
            }
            if (!IsPng(inFile))
            {
                Printf("not png" + inFile.FullName);
                if (copyNotPng)
                {
                    CopyFile(inFile, outFile);
                }
                return;
            }
            var source = Load(inFile);
            if(source.Width %4 ==0 && source.Height %4 == 0)
            {
                if (copyMul4Png)
                {
                    RebuildPng(inFile, outFile);
                }
            }
            else
            {
                var convert = Convert(source);
                SaveTexture(convert, outFile);
                convert.Dispose();
                source.Dispose();
            }
        }
        public void SaveTexture(Bitmap textrue, FileInfo file)
        {
            textrue.Save(file.FullName, ImageFormat.Png);
        }
        public Bitmap Convert(FileInfo file)
        {
            return Convert(Load(file));
        }
        public Bitmap Convert(Bitmap source)
        {
            var oldw = source.Width;
            var oldh = source.Height;
            var neww = BiggerMul4(oldw);
            var newh = BiggerMul4(oldh);
            var xofst = (neww - oldw) / 2;
            var yofst = (newh - oldh) / 2;
            var result = new Bitmap(neww, newh);
            for (var x = 0; x < oldw; x++)
            {
                var newx = x + xofst;
                for (var y = 0; y < oldh; y++)
                {
                    var newy = y + yofst;
                    result.SetPixel(newx, newy, source.GetPixel(x, y));
                }
            }
            return result;
        }
        public int BiggerMul4(int size)
        {
            if (size % 4 == 0) return size;
            var r = size++;
            while (r % 4 != 0) r++;
            return r;
        }
        public bool IsPng(FileInfo file)
        {
            return file.Extension.ToLower() == ".png";
        }
        public bool IsDir(FileInfo file)
        {
            return (file.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }
        public Bitmap Load(FileInfo file)
        {
            return new Bitmap(file.FullName);
        }
        public string FormatPath(FileSystemInfo file)
        {
            return FormatPath(file.FullName);
        }
        public string FormatPath(string path)
        {
            return path.Replace('\\', '/');
        }
    }
}
