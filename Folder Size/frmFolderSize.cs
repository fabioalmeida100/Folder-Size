using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Folder_Size
{
    public partial class frmFolderSize : Form
    {
        private string _unidade;

        public frmFolderSize()
        {
            InitializeComponent();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();

            if (folderBrowser.SelectedPath != "")
            {
                txtPath.Text = folderBrowser.SelectedPath;
            }
        }    

        private void TreeFolder(string dir, TreeNode tv)
        {           

            if (dir.Length < 248)
            {
                string[] subdiretorioEntradas = Directory.GetDirectories(dir);
                
                foreach (string subdiretorio in subdiretorioEntradas)
                {
                    DirectoryInfo diretorio = new DirectoryInfo(subdiretorio);
                    TreeNode tds = tv.Nodes.Add(ObterStringFormatada(diretorio));
                    tds.StateImageIndex = 0;
                    tds.Tag = diretorio.FullName;
                    ProgressBar();
                    TreeFolder(subdiretorio, tds);                    
                }
            }
        }

        private double SizeOfFolder(DirectoryInfo dirInfo)
        {
            double size = 0;

            foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                var divisor = 1;

                if (rbMegaBytes.Checked)
                    divisor = 1000000;  

                size += fi.Length / divisor;          
            }

            return size;
        }

        private void ProgressBar()
        {
            if (progressBarFolder.Value < progressBarFolder.Maximum)
            {
                progressBarFolder.Value++;
                int percentual = (int)(((double)progressBarFolder.Value / (double)progressBarFolder.Maximum) * 100);
                progressBarFolder.CreateGraphics().DrawString(percentual.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black,
                    new PointF(progressBarFolder.Width / 2 - 10, progressBarFolder.Height / 2 - 7));
                Application.DoEvents();
            }
        }

        private string ObterDescricaoUnidade()
        {
            string unidade;

            if (rbMegaBytes.Checked)
               unidade = "MB";
            else
               unidade = "Bytes";

            return unidade;
        }

        private string ObterStringFormatada(DirectoryInfo diretorio)
        {
            return diretorio.Name + " | Tamanho: " + string.Format("{0:0,0}", SizeOfFolder(diretorio).ToString("0,0", CultureInfo.GetCultureInfo("pt-BR").NumberFormat)) + " (" + _unidade + ")";
        }

        private void btnCarregar_Click(object sender, EventArgs e)
        {
            if (txtPath.Text != "")
            {
                _unidade = ObterDescricaoUnidade();
                progressBarFolder.Maximum = Directory.GetDirectories(txtPath.Text, "**", SearchOption.AllDirectories).Length;
                DirectoryInfo diretorio = new DirectoryInfo(txtPath.Text);

                tvFolder.Nodes.Clear();
                TreeNode mainNode = tvFolder.Nodes.Add(ObterStringFormatada(diretorio));
                mainNode.Tag = diretorio.FullName;

                TreeFolder(txtPath.Text, mainNode);
            }
            else
            {
                MessageBox.Show("Selecione o diretório principal", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSelectFolder.Focus();
            }
        }

        private void frmFolderSize_Load(object sender, EventArgs e)
        {
            Text += " | v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
