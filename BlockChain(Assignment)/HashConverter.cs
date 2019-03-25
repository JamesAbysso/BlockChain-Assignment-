using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BlockChainAssignment
{
    static class HashConverter
    {
        public static HashAlgorithm CSP { get; set; } = new SHA1CryptoServiceProvider();

        public static int HASHLEN = CSP.HashSize >> 2;
        
        public static string HashGen(params string[] strings)
        {
            HASHLEN = CSP.HashSize >> 2;
            byte[] hash = new byte[CSP.HashSize >> 4];

            using (MemoryStream memStream = new MemoryStream())
            {
                foreach (string s in strings)
                {
                    byte[] dataArray = (new UnicodeEncoding()).GetBytes(s);
                    memStream.Write(dataArray, 0, dataArray.Length);
                }
                memStream.Seek(0, SeekOrigin.Begin);
                hash = CSP.ComputeHash(memStream);
            }

            StringBuilder builder = new StringBuilder(HASHLEN);
            foreach (byte b in hash) builder.AppendFormat("{0:x2}", b);
            return builder.ToString();
        }
    }
}
