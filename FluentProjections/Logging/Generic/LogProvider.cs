using System;

namespace FluentProjections.Logging.Generic
{
    public static class LogProvider<T1>
    {
        public static ILog<T1> GetLogger(Type type)
        {
            return new LogWrapper<T1>(LogProvider.GetLogger(type.FullName));
        }
    }

    public static class LogProvider<T1, T2>
    {
        public static ILog<T1, T2> GetLogger(Type type)
        {
            return new LogWrapper<T1, T2>(LogProvider.GetLogger(type.FullName));
        }
    }
}