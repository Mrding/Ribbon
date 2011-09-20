using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Luna.Common
{
    public static class PassWord
    {
        private static byte[] key = ASCIIEncoding.ASCII.GetBytes("qwer/.,m");
        private static byte[] iv = ASCIIEncoding.ASCII.GetBytes("9ijn6tfc");

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="inputString">輸入字符</param>
        /// <returns>加密後的字符</returns>
        public static string DESEncrypt(string inputString)
        {
            if (inputString.Trim() == string.Empty)
                return "";
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamWriter sw = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                sw = new StreamWriter(cs);
                sw.Write(inputString);
                sw.Flush();
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="inputString">輸入字符</param>
        /// <returns>解密後的字符</returns>
        public static string DESDecrypt(string inputString)
        {
            if (inputString.Trim() == string.Empty)
                return "";
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamReader sr = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream(Convert.FromBase64String(inputString));
                cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            finally
            {
                if (sr != null) sr.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }
    }
}
