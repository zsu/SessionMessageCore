/// Author: Zhicheng Su
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SessionMessages.Core
{
    public class SessionStateSessionMessageProvider:ISessionMessageProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        #region ISessionMessageProvider Members
        public SessionStateSessionMessageProvider(IHttpContextAccessor httpContextAccessor)
        { _httpContextAccessor = httpContextAccessor; }
        public void SetMessage(SessionMessage message)
        {
            if (message == null)
                return;
            List<SessionMessage> messages = GetMessage();
            if (messages == null)
                messages = new List<SessionMessage>();
            if (!string.IsNullOrEmpty(message.Key) && messages.Exists(x => x.Key == message.Key && x.Behavior == message.Behavior))
                return;
            messages.Add(message);
            _httpContextAccessor.HttpContext.Session.SetString(SessionMessageManager.SessionMessageKey, JsonConvert.SerializeObject(messages));
        }
        public List<SessionMessage> GetMessage()
        {
            var sessionString = _httpContextAccessor.HttpContext.Session.GetString(SessionMessageManager.SessionMessageKey);
            List<SessionMessage> sessionMessages = sessionString==null ? null : JsonConvert.DeserializeObject<List<SessionMessage>>(sessionString);
            return sessionMessages;
        }

        public void Clear()
        {
            _httpContextAccessor.HttpContext.Session.Remove(SessionMessageManager.SessionMessageKey);
        }

        #endregion
    }
}
