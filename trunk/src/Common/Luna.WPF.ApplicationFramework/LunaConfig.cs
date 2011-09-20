using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Luna.Common;
using System.IO;
using Caliburn.Core;
using System.Reflection;
using Luna.Common.Network;

namespace Luna.WPF.ApplicationFramework
{
    [XmlType]
    public class LunaConfig : PropertyChangedBase
    {


        #region General

        //启动时
        [XmlElement]
        public bool AutoRun { get; set; }

        [XmlElement]
        public bool AutoLogin { get; set; }

        //自动更新
        [XmlElement]
        public bool AutoUpdate { get; set; }


        //主面板
        [XmlElement]
        public bool IsTopmost { get; set; }

        [XmlElement]
        public CloseWindowMode CloseWindowMode { get; set; }

        #endregion

       
        [XmlElement]
        public Proxy Proxy { get; set; }

        #region Method


        private void Nofity()
        {
            var infos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in infos)
            {
                NotifyOfPropertyChange(item.Name);
            }
        }

        public void Reset()
        {
            SetDefaultOption();
            Nofity();
        }

        public void Cancel()
        {
            var config = CreateConfig();
            this.AutoRun = config.AutoRun;
            this.AutoLogin = config.AutoLogin;
            this.AutoUpdate = config.AutoUpdate;
            this.IsTopmost = config.IsTopmost;
            this.CloseWindowMode = config.CloseWindowMode;
            this.Proxy = config.Proxy;
            Nofity();
        }

        private void SetDefaultOption()
        {
            this.AutoRun = false;
            this.AutoLogin = false;
#if DEBUG
            this.AutoUpdate = false;
#else
            this.AutoUpdate = true;
#endif
            this.IsTopmost = false;
            this.CloseWindowMode = CloseWindowMode.Close;
            this.Proxy = new Proxy();
            this.Proxy.ProxyType = ProxyType.None;
            this.Proxy.Username= string.Empty;
            this.Proxy.Password = string.Empty;
            this.Proxy.IP = string.Empty;
            this.Proxy.Port= string.Empty;
            this.Proxy.Domain = string.Empty;
        }

        private LunaConfig()
        {
        }

        private static LunaConfig _appConfig;
        public static LunaConfig Current
        {
            get
            {
                if (Caliburn.PresentationFramework.PresentationFrameworkModule.IsInDesignMode) return null;
                if (_appConfig == null)
                {
                    _appConfig = LunaConfig.CreateConfig();
                }
                return _appConfig;
            }
        }
        //Data//
        public static string CONFIGPATH = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";

        public void Save()
        {
            IXmlSerializer serializer = new DefaultXmlSerializer();
            serializer.Serialize(this, CONFIGPATH);
            Nofity();
        }

        private static LunaConfig CreateConfig()
        {
            EnsureFilePath();
            IXmlSerializer serializer = new DefaultXmlSerializer();
            return serializer.Deserialize<LunaConfig>(CONFIGPATH);
        }

        private static void EnsureFilePath()
        {

            if (!new FileInfo(CONFIGPATH).Exists)
            {
                LunaConfig config = new LunaConfig();
                
                config.SetDefaultOption();
                config.Save();
            }
        }

        #endregion
    }

    public enum CloseWindowMode
    {
        Mini = 0,
        Close = 1
    }

    public enum ProxyTypes
    {
        None,
        Http,
        Socket4,
        Socket5
    }
}
