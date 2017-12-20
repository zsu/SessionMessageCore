namespace SessionMessages.Core
{
    public interface ISessionMessageFactory
    {
        ISessionMessageProvider CreateInstance();
    }
}