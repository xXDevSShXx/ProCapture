using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using ProCapture.Models;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Configuration;

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

        private void Form_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Ready";
            titleLabel.Text = "Pro Capture";
            Invalidate();

            FolderTextBox.Text = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\.minecraft";
        }
        private string mcPath;
        private async void scanButton_Click(object sender, EventArgs e)
        {
            scanButton.Enabled = false;
            mcPath = string.IsNullOrEmpty(FolderTextBox.Text) ? FolderTextBox.PlaceholderText : FolderTextBox.Text;
            if (!(Directory.Exists(mcPath)) || !(Directory.Exists(mcPath + @"\versions")))
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
                Invalidate();
                await Task.Run(async () =>
                {
                    Dictionary<string, Object> jsonData = new Dictionary<string, Object>();
                    jsonData.Add("UID", Environment.UserName);

                    List<string> mods = new List<string>();
                    List<object> modNames = new List<object>();
                    List<string> versions = new List<string>();
                    List<string> versionNames = new List<string>();
                    List<string> modfolders = new List<string>();

                    if (Directory.Exists(mcPath + @"\versions"))
                    {
                        versions.AddRange(Directory.GetDirectories(mcPath + @"\versions").ToList());
                        foreach (var item in versions)
                        {
                            versionNames.Add(new DirectoryInfo(item).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
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
                            var folder = new StringBuilder();
                            folder.Append(new DirectoryInfo(item).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
                            folder.Append(";");
                            foreach (var itemInItem in Directory.GetFiles(item))
                            {
                                folder.Append(new FileInfo(itemInItem).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
                                folder.Append(";");
                            }
                            modfolders.Add(folder.ToString());
                        }
                    }

                    var desktop = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    var downloads = Directory.GetFiles($@"{Environment
                        .GetFolderPath(Environment.SpecialFolder.UserProfile)}\Downloads");

                    var desktopFileNames = new List<string>();
                    var downloadNames = new List<string>();
                    var desktopfolders = new List<string>();
                    var downloadfolders = new List<string>();

                    foreach (var item in desktop)
                    {
                        desktopFileNames.Add(new FileInfo(item).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
                    }
                    foreach (var item in Directory.GetDirectories(Environment
                        .GetFolderPath(Environment.SpecialFolder.Desktop)))
                    {
                        var folder = new StringBuilder();
                        folder.Append(new DirectoryInfo(item).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
                        folder.Append(";");
                        foreach (var itemInItem in Directory.GetFiles(item))
                        {
                            folder.Append(new FileInfo(itemInItem).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
                            folder.Append(";");
                        }
                        desktopfolders.Add(folder.ToString());
                    }

                    foreach (var item in downloads)
                    {
                        downloadNames.Add(new FileInfo(item).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
                    }
                    foreach (var item in Directory.GetDirectories($@"{Environment
                        .GetFolderPath(Environment.SpecialFolder.UserProfile)}\Downloads"))
                    {
                        var folder = new StringBuilder();
                        folder.Append(new DirectoryInfo(item).Name);
                        folder.Append(";");
                        foreach (var itemInItem in Directory.GetFiles(item))
                        {
                            folder.Append(new FileInfo(itemInItem).Name.Replace("\'", "\\\'").Replace("\"", "\\\""));
                            folder.Append(";");
                        }
                        downloadfolders.Add(folder.ToString());
                    }

                    List<string> pathes = new List<string>();
                    foreach (var item in desktop)
                    {
                        pathes.Add(new FileInfo(item).FullName);
                    }
                    foreach (var item in Directory.GetDirectories(Environment
                        .GetFolderPath(Environment.SpecialFolder.Desktop)))
                    {
                        foreach (var itemInItem in Directory.GetFiles(item))
                        {
                            pathes.Add(new FileInfo(itemInItem).FullName);
                        }
                    }

                    //foreach (var item in downloads)
                    //{
                    //    pathes.Add(new FileInfo(item).FullName);
                    //}
                    //foreach (var item in Directory.GetDirectories($@"{Environment
                    //    .GetFolderPath(Environment.SpecialFolder.UserProfile)}\Downloads"))
                    //{
                    //    foreach (var itemInItem in Directory.GetFiles(item))
                    //    {
                    //        pathes.Add(new FileInfo(itemInItem).FullName);
                    //    }
                    //}
                    var ghostClients = FileScanner.Scan(pathes);

                    jsonData.Add("desktop", desktopFileNames);
                    jsonData.Add("downloads", downloadNames);
                    jsonData.Add("versions", versionNames);
                    jsonData.Add("mods", modNames);
                    jsonData.Add("modfolders", modfolders);
                    jsonData.Add("desktopfolders", desktopfolders);
                    jsonData.Add("downloadfolders", downloadfolders);
                    jsonData.Add("ghostclients", ghostClients);

                    string json = JsonConvert.SerializeObject(jsonData);

                    var r = await MakeRequest(json);
                    if (r.IsSuccess)
                        Invoke((Action)(() =>
                        {
                            isWorking = false;
                            statusLabel.Text = "Ready";
                            (new ShowIdForm(r.Entity)).ShowDialog(this);
                        }));
                    Invoke((Action)(() =>
                    {
                        scanButton.Enabled = true;
                    }));

                });

            }
            catch (IOException)
            {
                MessageBox.Show("Something Went Wrong. It Is About Files And Folders Probably.");
            }
            catch (Exception ex)
            {
                Clipboard.SetText(ex.Message);
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                isWorking = false;
                statusLabel.Text = "Ready";
                Invalidate();
            }

        }

        private async Task<ResponseBase<string>> MakeRequest(string json)
        {
            return await Task.Run(async () =>
            {
                var source = new CancellationTokenSource();
                var cancellationToken = source.Token;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {


                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var result = await client.PostAsync(ConfigurationManager.AppSettings.Get("InsertUrl"), content
                            , cancellationToken);

                        return new ResponseBase<string>(true, await result.Content.ReadAsStringAsync());
                    }
                }
                catch (HttpRequestException)
                {
                    source.Cancel();
                    MessageBox.Show($"Could Not Connect To Servers. Please Check Your Internet Connection.", "Error");
                    return new ResponseBase<string>(false, null);
                }
                catch (Exception ex)
                {
                    source.Cancel();
                    MessageBox.Show(ex.Message, "Error");
                    return new ResponseBase<string>(false, null);
                }
            });
        }
    }

}
