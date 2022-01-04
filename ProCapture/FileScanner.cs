using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ProCapture
{
    public static class FileScanner
    {

        public static void Initialize(Dictionary<string, string> signatures) => Signatures = signatures;

        private static string GetMD5FromFile(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }

        public static Dictionary<string, string> Signatures;
        public static List<string> Scan(List<string> filePathes)
        {
            var ghostClients = new List<string>();
            foreach (var item in filePathes)
            {
                string file = GetMD5FromFile(item);
                foreach (var signature in Signatures)
                {
                    if (signature.Value.Contains(file))
                        ghostClients.Add(signature.Key);
                }
            }
            return ghostClients;
        }
    }
}
