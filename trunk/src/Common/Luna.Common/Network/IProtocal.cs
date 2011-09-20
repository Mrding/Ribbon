using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Luna.Common.Network
{
    public interface IProtocal
    {
        void SetProxy(Proxy proxy);
        void Transport(string url);
    }

    public delegate void ProxyEventHandler<TEventArgs>(Object sender, TEventArgs e);

    public class WebProxyProtocal : IProtocal
    {
        private IWebProxy _proxy;
        private HttpWebRequest _webRequest;
        public void SetProxy(Proxy proxy)
        {
            _proxy = ProxyFactory.CreateProxy(proxy);
        }

        public event ProxyEventHandler<HttpWebRequest> BeforeResponse;
        public event ProxyEventHandler<WebException> ErrorResponse;
        public event ProxyEventHandler<HttpWebResponse> AfterResponse;

        private void CheckProxy()
        {
            if (_proxy != null)
                _webRequest.Proxy = _proxy;
        }

        private void DoResponse()
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)_webRequest.GetResponse();
                if (AfterResponse != null)
                    AfterResponse(this, response);
                response.Close();
            }
            catch (WebException e)
            {
                if (ErrorResponse != null)
                    ErrorResponse(this, e);

            }
        }

        public void Transport(string url)
        {
            _webRequest = WebRequest.Create(url) as HttpWebRequest;
            CheckProxy();
            if (BeforeResponse != null)
                BeforeResponse(this, _webRequest);
            DoResponse();
        }
    }

    public class ProxyFactory
    {
        public static IWebProxy CreateProxy(Proxy entity)
        {
            var proxy = new WebProxy(entity.IP,int.Parse(entity.Port));
            proxy.Credentials = new NetworkCredential(entity.Username, entity.Password);
            return proxy;
        }
    }
}
