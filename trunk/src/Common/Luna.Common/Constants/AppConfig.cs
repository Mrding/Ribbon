using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Luna.Common.Constants
{
    public static class AppConfig
    {
        //<add key="Login" value="Admin"/> <!--可选值：LDAP(使用LDAP验证)、SSO(使用SSO验证)、NoPassword(无修改密码)、Password(修改密码)-->
        public static string LoginKey
        {
            get { return ConfigurationManager.AppSettings["Login"]; }
        }
        public static string LicenseKey
        {
            get { return ConfigurationManager.AppSettings["LicenseKey"]; }
        }
        public static bool IsLdap
        {
            get { return HasLoginConfigValue("LDAP"); }
        }

        public static bool IsSso
        {
            get { return HasLoginConfigValue("SSO"); }
        }

        public static bool IsAdmin
        {
            get { return HasLoginConfigValue("Admin"); }
        }

        public static bool IsNoPassword
        {
            get { return HasLoginConfigValue("NoPassword"); }
        }

        public static bool IsPassword
        {
            get { return HasLoginConfigValue("Password"); }
        }

        public static bool EnableProfiler
        {
            get { return HasConfigValue("EnableProfiler", bool.TrueString); }
        }

        public static bool HasAuthRole
        {
            get { return HasConfigValue("HasAuthRole", bool.TrueString); }
        }

        public static bool HasLoginConfigValue(string value)
        {
            return HasConfigValue("Login", value);
        }

        public static bool HasConfigValue(string appKey, string value)
        {
            var configValue = ConfigurationManager.AppSettings[appKey];
            return !String.IsNullOrEmpty(configValue) && configValue.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        //public static string ConnectionString
        //{
        //    get { return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; }
        //}

        public static string ConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                if (!connectionString.ToLower().Contains("server"))
                    connectionString = GetDesDecrypt(connectionString);
                return connectionString;
            }
        }

        private static string GetDesDecrypt(string decryptConnectionString)
        {
            byte[] key = Encoding.ASCII.GetBytes("qwer/.,m");
            byte[] iv = Encoding.ASCII.GetBytes("9ijn6tfc");
            if (decryptConnectionString.Trim() == string.Empty)
                return "";
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamReader sr = null;

            var des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream(Convert.FromBase64String(decryptConnectionString));
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

        public static string ExtConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["ExtConnectionString"].ConnectionString; }
        }

        public static int SSORetrySeconds
        {
            get
            {
                var second = ConfigurationManager.AppSettings["SSORetrySeconds"];
                if (string.IsNullOrEmpty(second))
                    return 10;
                return int.Parse(second);
            }
        }
    }
}
