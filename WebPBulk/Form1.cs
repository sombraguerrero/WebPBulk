//Note: Imazen WEBP is only compatible with 32-bit libraries using version 1.2.4 or below. 1.3.0+ breaks it.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Imazen.WebP;

namespace WebPBulk
{
    public partial class Form1 : Form
    {
        private static bool doBulk;
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            label1.Text += trackBar1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                progressBar1.Visible = doBulk = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (doBulk && radioButton1.Checked)
            {
                EncodeOut();
            }
            else if (doBulk && radioButton2.Checked)
            {
                DecodeOut();
            }
            else if (!doBulk && radioButton1.Checked)
            {
                EncodeFileOut();
            }
            else if (!doBulk && radioButton2.Checked)
            {
                DecodeFileOut();
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
                        simpleEncoder.Encode(bitmap, fs, trackBar1.Value);
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
                    bitmap.Save($"{targetDir}\\{item.Name.Remove(item.Name.Length - extLen, extLen)}bmp");
                    progressBar1.PerformStep();
                }
                else
                    progressBar1.Maximum--;
            }
        }

        private void filesBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                progressBar1.Visible = doBulk = false;
            }
        }
        private void EncodeFileOut()
        {
            SimpleEncoder simpleEncoder = new SimpleEncoder();
            Bitmap bitmap = new Bitmap(textBox1.Text);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = File.Create(saveFileDialog1.FileName))
                {
                    simpleEncoder.Encode(bitmap, fs, trackBar1.Value);
                    MessageBox.Show($"File encoded to {saveFileDialog1.FileName}", "Complete");
                }
            }
        }

        private void DecodeFileOut()
        {
            SimpleDecoder simpleDecoder = new SimpleDecoder();
            byte[] imageData = File.ReadAllBytes(textBox1.Text);
            saveFileDialog1.DefaultExt = "bmp";
            saveFileDialog1.Filter = "Bitmap Files|*.bmp";
            Bitmap bitmap = simpleDecoder.DecodeFromBytes(imageData, imageData.Length);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(saveFileDialog1.FileName);
                MessageBox.Show($"File decoded to {saveFileDialog1.FileName}", "Complete");
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = $"Quality: {trackBar1.Value}";
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fn = (string[])e.Data.GetData(DataFormats.FileDrop);
            textBox1.Text = fn[0];
            progressBar1.Visible = doBulk = Directory.Exists(textBox1.Text);
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
    }
}
