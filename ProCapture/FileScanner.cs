using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ProCapture
{
    public static class FileScanner
    {

        public static void Initialize(string signatures) => Signatures = signatures;

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

        public static string Signatures;
        public static List<string> Scan(List<string> filePathes)
        {
            var susFiles = new List<string>();
            foreach (var item in filePathes)
            {
                string file = GetMD5FromFile(item);

                if (Signatures.Contains(file))
                {
                    susFiles.Add(item);
                }
            }
            return susFiles;
        }
    }
}
