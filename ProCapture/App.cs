using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;

namespace ProCapture
{
    public static class App
    {
        private static string DefaultJson => "{\"ThemeColor\": {\"r\": \"92\",\"g\": \"28\",\"b\": \"150\"}}";



        public static async void Initialize(string path, string jsonFile)
        {
            if (!File.Exists($"{path}Config.bin")) 
                MessageBox.Show("Configuration File Missing. Please Re-Install Application");

            if (!File.Exists($"{path}{jsonFile}"))
            {
                var file = File.CreateText($"{path}{jsonFile}");
                file.Write(DefaultJson);
            }

            Configurations = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile(jsonFile,false,true)
            .Build();

            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var result = await client.GetAsync(Properties.Settings.Default.GetUrl , cancellationToken);
                    var signatures = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        await result.Content.ReadAsStringAsync()
                        );
                    FileScanner.Initialize(signatures);
                }
            }
            catch (HttpRequestException)
            {
                source.Cancel();
                MessageBox.Show($"Could Not Connect To Servers. Please Check Your Internet Connection.", "Error");
            }

        }
        public static IConfigurationRoot Configurations { get; internal set; }
    }
}
