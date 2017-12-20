using System.Collections.Generic;

namespace SessionMessages.Core
{
    public interface ISessionMessageProvider
    {
        void SetMessage(SessionMessage message);
        List<SessionMessage> GetMessage();
        void Clear();
    }
}
