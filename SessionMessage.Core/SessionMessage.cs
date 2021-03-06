/// Author: Zhicheng Su
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
namespace SessionMessage.Core
{
    public class SessionMessageManager : ISessionMessageManager
    {
        public const string SessionMessageKey = "SessionMessage";
        private ISessionMessageProvider _provider;
        private ISessionMessageFactory _sessionMessageFactory;

        public SessionMessageManager(ISessionMessageFactory sessionMessageFactory) {
            _sessionMessageFactory = sessionMessageFactory;
            _provider = sessionMessageFactory.CreateInstance();
        }
        public List<SessionMessage> GetMessage()
        {
            return _provider.GetMessage();
        }
        public void SetMessage(MessageType messageType, MessageBehaviors behavior, string message)
        {
            SetMessage(messageType, behavior, message, null, null, null, null);
        }
        /// <summary>
        /// Set message. message with key only display once not matter how many ajax calls on the same page.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="behavior"></param>
        /// <param name="message"></param>
        /// <param name="key"></param>
        public void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, string key)
        {
            SetMessage(messageType, behavior, message, key, null, null, null);
        }
        public void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, string key,string closeCallback)
        {
            SetMessage(messageType, behavior, message, key, null, null, null,closeCallback);
        }
        public void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, MessageButton? messageButtons)
        {
            SetMessage(messageType, behavior, message, null, null, messageButtons, null);
        }
        public void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, string key, string caption, MessageButton? messageButtons, MessageIcon? messageIcon)
        {
            SetMessage(messageType, behavior, message, key, caption, messageButtons, messageIcon,null);
        }
        public void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, string key, string caption, MessageButton? messageButtons, MessageIcon? messageIcon,string closeCallback)
        {
            //if (caption == null || caption.Trim() == string.Empty)
            //    caption = messageType.ToString();
            if (!messageButtons.HasValue)
                messageButtons = MessageButton.Ok;
            if (!messageIcon.HasValue)
            {
                switch (messageType)
                {
                    case MessageType.Error:
                        messageIcon = MessageIcon.Error;
                        break;
                    case MessageType.Warning:
                        messageIcon = MessageIcon.Warning;
                        break;
                    case MessageType.Info:
                        messageIcon = MessageIcon.Information;
                        break;
                }
            }
            SessionMessage sessionMessage = new SessionMessage(messageType, behavior, message, key, caption, messageButtons, messageIcon,closeCallback);
            _provider.SetMessage(sessionMessage);
        }
        public void Clear()
        {
            _provider.Clear();
        }
    }
    /// <summary>
    /// Summary description for SessionMessage
    /// </summary>
    [DataContract]
    public class SessionMessage
    {
        [DataMember]
        public string Caption
        {
            get; set;
        }
        [DataMember]
        public string Message
        {
            get; set;
        }
        [DataMember]
        public MessageType Type
        {
            get; set;
        }
        [DataMember]
        public MessageBehaviors Behavior
        {
            get; set;
        }
        [DataMember]
        public MessageButton? Buttons
        {
            get; set;
        }
        [DataMember]
        public MessageIcon? Icon
        {
            get; set;
        }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string CloseCallBack { get; set; }
        public SessionMessage(MessageType messageType, MessageBehaviors behavior, string message)
            : this(messageType, behavior, message, null, null, null, null,null)
        {
        }
        public SessionMessage(MessageType messageType, MessageBehaviors behavior, string message, string key, string caption, MessageButton? messageButtons, MessageIcon? messageIcon,string closeCallback)
        {
            if (behavior == MessageBehaviors.Modal && (!messageButtons.HasValue || !messageIcon.HasValue))
            {
                if (behavior != MessageBehaviors.Modal && !string.IsNullOrWhiteSpace(closeCallback))
                    throw new ArgumentException("{0} only available for Modal Dialog.", nameof(closeCallback));
                messageButtons = messageButtons ?? MessageButton.Ok;
                if (!messageIcon.HasValue)
                {
                    switch (messageType)
                    {
                        case MessageType.Error:
                            messageIcon = MessageIcon.Error;
                            break;
                        case MessageType.Info:
                            messageIcon = MessageIcon.Information;
                            break;
                        case MessageType.Success:
                            messageIcon = MessageIcon.Success;
                            break;
                        case MessageType.Warning:
                            messageIcon = MessageIcon.Warning;
                            break;
                        default:
                            messageIcon = MessageIcon.Information;
                            break;
                    }
                }
            }
            Key = key;
            Message = message;
            Caption = caption;
            Type = messageType;
            Behavior = behavior;
            Buttons = messageButtons;
            Icon = messageIcon;
            CloseCallBack = closeCallback;
        }
    }

    public enum MessageButton
    {
        Ok = 0
        //OKCancel = 1,
        //AbortRetryIgnore = 2,
        //YesNoCancel = 3,
        //YesNo = 4,
        //RetryCancel = 5
    }
    public enum MessageIcon
    {
        None = 0,
        Error = 1,
        Hand = 2,
        Stop = 3,
        Lock = 4,
        Question = 5,
        Exclamation = 6,
        Warning = 7,
        Asterisk = 8,
        Information = 9,
        Success = 10
    }

    [Flags]
    public enum MessageBehaviors
    {
        StatusBar = 1,
        Modal = 2
    }
    public enum MessageType
    {
        Error = 1,
        Warning = 2,
        Info = 3,
        Success = 4
    }
}


