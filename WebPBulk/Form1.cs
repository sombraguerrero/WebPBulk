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
                Bitmap bitmap = new Bitmap(item.FullName);
                using (FileStream fs = File.Create($"{targetDir}\\{item.Name.Remove(item.Name.Length - 3, 3)}webp"))
                {
                    simpleEncoder.Encode(bitmap, fs, 85);
                    progressBar1.PerformStep();
                }
            }
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
                byte[] imageData = File.ReadAllBytes(item.FullName);
                Bitmap bitmap = simpleDecoder.DecodeFromBytes(imageData, imageData.Length);
                bitmap.Save($"{targetDir}\\{item.Name.Remove(item.Name.Length - 4, 4)}png");
                progressBar1.PerformStep();
            }
        }
    }
}
