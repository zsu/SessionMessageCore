using System.Collections.Generic;

namespace SessionMessage.Core
{
    public interface ISessionMessageManager
    {
        void Clear();
        List<SessionMessage> GetMessage();
        void SetMessage(MessageType messageType, MessageBehaviors behavior, string message);
        void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, MessageButton? messageButtons);
        void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, string key);
        void SetMessage(MessageType messageType, MessageBehaviors behavior, string message, string key, string caption, MessageButton? messageButtons, MessageIcon? messageIcon);
    }
}