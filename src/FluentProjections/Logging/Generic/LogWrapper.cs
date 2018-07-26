using System;

namespace FluentProjections.Logging.Generic
{
    public class LogWrapper<T1> : ILog<T1>
    {
        private readonly ILog _log;

        public LogWrapper(ILog log)
        {
            _log = log;
        }

        public void Log(LogLevel logLevel, Func<string> messageFunc)
        {
            _log.Log(logLevel, messageFunc);
        }

        public void Log<TException>(LogLevel logLevel, Func<string> messageFunc, TException exception)
            where TException : Exception
        {
            _log.Log(logLevel, messageFunc, exception);
        }
    }

    public class LogWrapper<T1, T2> : ILog<T1, T2>
    {
        private readonly ILog _log;

        public LogWrapper(ILog log)
        {
            _log = log;
        }

        public void Log(LogLevel logLevel, Func<string> messageFunc)
        {
            _log.Log(logLevel, messageFunc);
        }

        public void Log<TException>(LogLevel logLevel, Func<string> messageFunc, TException exception)
            where TException : Exception
        {
            _log.Log(logLevel, messageFunc, exception);
        }
    }
}