using System.Collections.Generic;

namespace SessionMessage.Core
{
    public interface ISessionMessageProvider
    {
        void SetMessage(SessionMessage message);
        List<SessionMessage> GetMessage();
        void Clear();
    }
}
