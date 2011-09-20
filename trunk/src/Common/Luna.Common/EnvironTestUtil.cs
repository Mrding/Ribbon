using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using Luna.Common.Network;

namespace Luna.Common
{
    public interface IDbConnectionTest
    {
        bool Test(string connString);
    }

    public class EnvironTestUtil
    {

        public static bool TestSqlDbConnection(string connString)
        {
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                return false;

            }
            conn.Close();
            conn.Dispose();
            return true;
        }

        public static bool TestNHDbConnection()
        {
            string connstring = GetConnectionString();
            return EnvironTestUtil.TestSqlDbConnection(connstring);
        }

        public static string GetConnectionString()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "hibernate.cfg.xml");
            return doc.DocumentElement.FirstChild.ChildNodes.Cast<XmlNode>().First(e => e.Attributes["name"].Value == "connection.connection_string").InnerText;
        }

        public static bool TestNetWork(string url)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //myHttpWebRequest.Timeout = 10000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
                response.Close();
            }
            catch (Exception e)
            {
                return false;
                // throw;
            }
            return true;
        }

        public static bool TestUpdateURL()
        {
            return TestNetWork(ConfigurationManager.AppSettings["updateURL"] + "//root.xml");

        }

    }
}
