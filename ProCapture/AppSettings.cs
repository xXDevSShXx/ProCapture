using Microsoft.Extensions.Configuration;

namespace ProCapture
{
    public static class AppSettings
    {
        public static void Initialize(string path,string jsonFile)
        {
            Settings = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile(jsonFile)
            .Build();
        }
        public static IConfigurationRoot Settings { get; internal set; }
    }
}
