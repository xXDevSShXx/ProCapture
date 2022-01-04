using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProCapture
{
    public partial class ShowIdForm : Form
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

        public ShowIdForm(string Id)
        {
            InitializeComponent();
            idTextBox.Text = Id;
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Invoke((Action)(() => { Close(); }));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(Color.FromArgb(
                              Convert.ToInt32(App.Settings["ThemeColor:r"])
                            , Convert.ToInt32(App.Settings["ThemeColor:g"])
                            , Convert.ToInt32(App.Settings["ThemeColor:b"])
                            ), 2);
            Rectangle rectangle = new Rectangle(0, 0, Width, Height);
            graphics.DrawRectangle(pen, rectangle);
            base.OnPaint(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            idTextBox.Copy();
        }

        private void ShowIdForm_Load(object sender, EventArgs e)
        {

        }
    }
}
