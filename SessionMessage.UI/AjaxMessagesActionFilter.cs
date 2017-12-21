/// Author: Zhicheng Su
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.Filters;
using SessionMessage.Core;

namespace SessionMessage.UI
{
    /// <summary>
    /// If we're dealing with ajax requests, any message that is in the view data goes to
    /// the http header.
    /// </summary>
    public class AjaxMessagesActionFilter : IActionFilter
    {
        private ISessionMessageManager _sessionMessageManager;
        public AjaxMessagesActionFilter(ISessionMessageManager sessionMessageManager)
        {
            _sessionMessageManager = sessionMessageManager;
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                var response = filterContext.HttpContext.Response;
                List<Core.SessionMessage> sessionMessages = _sessionMessageManager.GetMessage();
                if (sessionMessages != null && sessionMessages.Count > 0)
                {
                    string json = null;
                    var messages = sessionMessages.Where(x => x.Behavior == MessageBehaviors.StatusBar).Select(x => new SessionMessageJsonModal { Message = x.Message, Type = Enum.GetName(typeof(MessageType), x.Type),Key=x.Key,Caption=x.Caption }).ToList();
                    if (messages != null && messages.Count > 0)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(messages.GetType());
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ser.WriteObject(ms, messages);
                            json = Encoding.Default.GetString(ms.ToArray());
                            ms.Close();
                        }
                        response.Headers.Add("X-Message", json);
                    }
                    messages = sessionMessages.Where(x => x.Behavior == MessageBehaviors.Modal).Select(x => new SessionMessageJsonModal { Message = x.Message, Type = Enum.GetName(typeof(MessageType), x.Type), Key=x.Key }).ToList();
                    if (messages != null && messages.Count > 0)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(messages.GetType());
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ser.WriteObject(ms, messages);
                            json = Encoding.Default.GetString(ms.ToArray());
                            ms.Close();
                        }
                        response.Headers.Add("X-ModalMessage", json);
                    }
                    _sessionMessageManager.Clear();
                }
            }
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
        [DataContract]
        private class SessionMessageJsonModal
        {
            [DataMember]
            public string Message
            {
                get;
                set;
            }
            [DataMember]
            public string Type
            {
                get;
                set;
            }
            [DataMember]
            public string Key
            {
                get;
                set;
            }
            [DataMember]
            public string Caption
            {
                get;
                set;
            }
        }
    }
 }