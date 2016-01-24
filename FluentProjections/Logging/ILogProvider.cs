namespace FluentProjections.Logging
{
    public interface ILogProvider
    {
        ILog GetLogger(string name);
    }
}