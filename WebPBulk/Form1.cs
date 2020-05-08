using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Imazen.WebP;

namespace WebPBulk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                EncodeOut();
            }
            else
            {
                DecodeOut();
            }
        }

        private void EncodeOut()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(textBox1.Text);
            string targetDir = textBox1.Text + "\\output";
            SimpleEncoder simpleEncoder = new SimpleEncoder();
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            progressBar1.Maximum = directoryInfo.GetFiles().Length;
            foreach (FileInfo item in directoryInfo.EnumerateFiles())
            {
                if (!item.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    int extLen = RemoveLen(item.Extension.Length, 4);
                    Bitmap bitmap = new Bitmap(item.FullName);
                    using (FileStream fs = File.Create($"{targetDir}\\{item.Name.Remove(item.Name.Length - extLen, extLen)}.webp"))
                    {
                        simpleEncoder.Encode(bitmap, fs, 85);
                        progressBar1.PerformStep();
                    }
                }
                else
                    progressBar1.Maximum--;
            }
        }

        private int RemoveLen(int srcLength,  int targetLength)
        {
            if (srcLength < targetLength)
                return targetLength - 1;
            else if (srcLength > targetLength)
                return targetLength + 1;
            else
                return targetLength;
        }

        private void DecodeOut()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(textBox1.Text);
            string targetDir = textBox1.Text + "\\output";
            SimpleDecoder simpleDecoder = new SimpleDecoder();
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            progressBar1.Maximum = directoryInfo.GetFiles().Length;
            foreach (FileInfo item in directoryInfo.EnumerateFiles())
            {
                if (!item.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    int extLen = RemoveLen(item.Extension.Length, 3);
                    byte[] imageData = File.ReadAllBytes(item.FullName);
                    Bitmap bitmap = simpleDecoder.DecodeFromBytes(imageData, imageData.Length);
                    bitmap.Save($"{targetDir}\\{item.Name.Remove(item.Name.Length - extLen, extLen)}png");
                    progressBar1.PerformStep();
                }
                else
                    progressBar1.Maximum--;
            }
        }
    }
}
