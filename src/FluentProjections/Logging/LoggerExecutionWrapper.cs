using System;

namespace FluentProjections.Logging
{
    public class LoggerExecutionWrapper : ILog
    {
        private const string FailedToGenerateLogMessage = "Failed to generate log message";
        private readonly ILog _logger;

        public LoggerExecutionWrapper(ILog logger)
        {
            _logger = logger;
        }

        public ILog WrappedLogger
        {
            get { return _logger; }
        }

        public void Log(LogLevel logLevel, Func<string> messageFunc)
        {
            Func<string> wrappedMessageFunc = () =>
            {
                try
                {
                    return messageFunc();
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, () => FailedToGenerateLogMessage, ex);
                }
                return null;
            };
            _logger.Log(logLevel, wrappedMessageFunc);
        }

        public void Log<TException>(LogLevel logLevel, Func<string> messageFunc, TException exception) where TException : Exception
        {
            Func<string> wrappedMessageFunc = () =>
            {
                try
                {
                    return messageFunc();
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, () => FailedToGenerateLogMessage, ex);
                }
                return null;
            };
            _logger.Log(logLevel, wrappedMessageFunc, exception);
        }
    }
}