using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;

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
            if (isWorking)
                if (MessageBox.Show(this, "The Scan Isn`t Complete Yet. Do You Want To Exit?", "ProCapture", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            Application.Exit();
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
            if (!(Directory.Exists(mcPath)))
            {
                MessageBox.Show("Minecraft Folder Not Found.\nPlease Enter You`r Minecraft Folders Path in That Text Box At The First Page.");
                return;
            }
            await Scan();
        }

        private bool isWorking = false;
        private async Task Scan()
        {
            try
            {
                isWorking = true;
                statusLabel.Text = "Scanning...";
                string id = await GetId(8);
                await Task.Run(() =>
                {
                    Dictionary<string, Object> jsonData = new Dictionary<string, Object>();
                    jsonData.Add("id", id);
                    jsonData.Add("UID", Environment.UserName);

                    List<string> mods = new List<string>();
                    List<object> modNames = new List<object>();
                    List<string> versions = new List<string>();
                    List<string> versionNames = new List<string>();

                    if (Directory.Exists(mcPath + @"\versions"))
                    {
                        versions.AddRange(Directory.GetDirectories(mcPath + @"\versions").ToList());
                        foreach (var item in versions)
                        {
                            versionNames.Add(new DirectoryInfo(item).Name);
                        }
                    }

                    if ((Directory.Exists(mcPath + @"\mods")))
                    {
                        mods.AddRange(Directory.GetFiles(mcPath + @"\mods").ToList());
                        foreach (var item in mods)
                        {
                            modNames.Add(new FileInfo(item).Name);
                        }
                        foreach (var item in Directory.GetDirectories(mcPath + @"\mods"))
                        {
                            var folder = new List<string>();
                            foreach (var itemInItem in Directory.GetFiles(item))
                            {
                                folder.Add(new FileInfo(itemInItem).Name);
                            }
                            modNames.Add(folder);
                        }
                    }

                    var desktop = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    var downloads = Directory.GetFiles($@"{Environment
                        .GetFolderPath(Environment.SpecialFolder.UserProfile)}\Downloads");

                    var desktopFileNames = new List<string>();
                    var downloadNames = new List<string>();

                    foreach (var item in desktop)
                    {
                        desktopFileNames.Add(new FileInfo(item).Name);
                    }
                    foreach (var item in Directory.GetDirectories(Environment
                        .GetFolderPath(Environment.SpecialFolder.Desktop)))
                    {
                        desktopFileNames.Add(new DirectoryInfo(item).Name);
                    }

                    foreach (var item in downloads)
                    {
                        downloadNames.Add(new FileInfo(item).Name);
                    }
                    foreach (var item in Directory.GetDirectories($@"{Environment
                        .GetFolderPath(Environment.SpecialFolder.UserProfile)}\Downloads"))
                    {
                        downloadNames.Add(new DirectoryInfo(item).Name);
                    }

                    jsonData.Add("desktop", desktopFileNames);
                    jsonData.Add("downloads", downloadNames);
                    jsonData.Add("versions", versionNames);
                    jsonData.Add("mods", modNames);

                    string json = JsonConvert.SerializeObject(jsonData);

                    MessageBox.Show(json);
                    //await MakeRequest(json);
                    Invoke((Action)(() =>
                    {
                        (new ShowIdForm(id)).ShowDialog(this);
                    }));


                });

            }
            catch (IOException)
            {
                MessageBox.Show("Something Went Wrong. It Is About Files And Folders Probably.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                isWorking = false;
                statusLabel.Text = "Ready";
            }

        }

        private async Task MakeRequest(string json)
        {
            await Task.Run(async () =>
            {
                var source = new CancellationTokenSource();
                var cancellationToken = source.Token;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var content = new StringContent(json);

                        var result = await client.PostAsync("https://pc.devmrz.ir/api/api.php", content
                            , cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    source.Cancel();
                    MessageBox.Show(ex.Message, "Error");
                }
            });
        }

        private async Task<string> GetId(int length)
        {
            return await Task.Run(() =>
            {
                StringBuilder builder = new StringBuilder();
                Random random = new Random();

                char letter;

                for (int i = 0; i < length; i++)
                {
                    double flt = random.NextDouble();
                    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                    letter = Convert.ToChar(shift + 65);
                    builder.Append(letter);
                }

                return builder.ToString();
            });
        }
    }

}
