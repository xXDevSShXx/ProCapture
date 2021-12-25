using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProCapture
{
    public partial class MainForm : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style = 0x20000;
                return cp;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "The Scan Isn`t Complete Yet. Do You Want To Exit?", "ProCapture", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        public string Title => "Pro Capture";
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(ThemeInfo.GetThemeColor(), 2);
            Rectangle rectangle = new Rectangle(0, 0, Width, Height);
            graphics.DrawRectangle(pen, rectangle);
            graphics.DrawImage(Icon.ToBitmap(), 5, 5, 24, 24);
            graphics.DrawString(Title, Font, new SolidBrush(Color.Snow), 30, 11);
            base.OnPaint(e);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            FolderTextBox.PlaceholderText = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\.minecraft";
        }
        private string mcPath;
        private async void scanButton_Click(object sender, EventArgs e)
        {
            mcPath = string.IsNullOrEmpty(FolderTextBox.Text) ? FolderTextBox.PlaceholderText : FolderTextBox.Text;
            if (!Directory.Exists(mcPath))
            {
                MessageBox.Show("Minecraft Folder Not Found.Enter A Valid Path");
                return;
            }
            await Scan();
        }

        private async Task Scan()
        {
            await Task.Run(() =>
            {
                Dictionary<string, Object> jsonData = new Dictionary<string, Object>();

                var versions = Directory.GetDirectories(mcPath + @"\versions").ToList();
                var mods = Directory.GetFiles(mcPath + @"\mods").ToList();

                var versionNames = new List<string>();
                var modNames = new List<string>();

                foreach (var item in versions)
                {
                    versionNames.Add(new DirectoryInfo(item).Name);
                }
                foreach (var item in mods)
                {
                    modNames.Add(new FileInfo(item).Name);
                }
                foreach (var item in Directory.GetDirectories(mcPath + @"\mods"))
                {
                    modNames.Add(new DirectoryInfo(item).Name);
                }
                jsonData.Add("versions", versionNames);
                jsonData.Add("mods", modNames);

                string json = JsonConvert.SerializeObject(jsonData);
                MessageBox.Show(json);
            });

        }
    }

}
