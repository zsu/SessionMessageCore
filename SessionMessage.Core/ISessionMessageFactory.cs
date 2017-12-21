namespace SessionMessage.Core
{
    public interface ISessionMessageFactory
    {
        ISessionMessageProvider CreateInstance();
    }
}