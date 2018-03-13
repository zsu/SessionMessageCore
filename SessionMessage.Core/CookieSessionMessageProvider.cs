/// Author: Zhicheng Su
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace SessionMessage.Core
{
    public class CookieSessionMessageProvider:ISessionMessageProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        #region ISessionMessageProvider Members
        public CookieSessionMessageProvider(IHttpContextAccessor httpContextAccessor)
        { _httpContextAccessor = httpContextAccessor; }
        public void SetMessage(SessionMessage message)
        {
            if (message == null)
                return;
            string json = null;
            List<SessionMessage> messages = GetMessage();
            if (messages == null)
                messages = new List<SessionMessage>();
            if (!string.IsNullOrEmpty(message.Key) && messages.Exists(x => x.Key == message.Key && x.Behavior==message.Behavior))
                return;
            messages.Add(message);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<SessionMessage>));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, messages);
                json = Encoding.Default.GetString(ms.ToArray());
                ms.Close();
            }
            _httpContextAccessor.HttpContext.Response.Cookies.Append(SessionMessageManager.SessionMessageKey, json);
        }
        public List<SessionMessage> GetMessage()
        {
            List<SessionMessage> message = null;
            var cookie = _httpContextAccessor.HttpContext.Request.Cookies[SessionMessageManager.SessionMessageKey];
            if(string.IsNullOrWhiteSpace(cookie))
                cookie=GetCookieValueFromResponse(_httpContextAccessor.HttpContext.Response,SessionMessageManager.SessionMessageKey);
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(cookie)))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<SessionMessage>));
                    message = ser.ReadObject(ms) as List<SessionMessage>;
                    ms.Close();
                }
            }
            return message;
        }

        public void Clear()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(SessionMessageManager.SessionMessageKey);
            //if (cookie != null)
            //    cookie.Expires=DateTime.Now.AddMinutes(-30); ;
        }
        private string GetCookieValueFromResponse(HttpResponse response, string cookieName)
        {
            foreach (var headers in response.Headers.Values)
                foreach (var header in headers)
                    if (header.StartsWith(cookieName))
                    {
                        var p1 = header.IndexOf('=');
                        var p2 = header.IndexOf(';');
                        return WebUtility.UrlDecode(header.Substring(p1 + 1, p2 - p1 - 1));
                    }
            return null;
        }
        #endregion
    }
}
