using System;
using System.Linq.Expressions;

namespace FluentProjections.Logging.LogProviders
{
    public class Log4NetLogProvider : ILogProvider
    {
        private readonly Func<string, object> _getLoggerByNameDelegate;

        public Log4NetLogProvider()
        {
            if (!IsLoggerAvailable()) throw new InvalidOperationException("log4net.LogManager not found");
            _getLoggerByNameDelegate = GetGetLoggerMethodCall();
        }

        public static bool ProviderIsAvailableOverride { get; set; } = true;

        public ILog GetLogger(string name)
        {
            return new Log4NetLogger(_getLoggerByNameDelegate(name));
        }

        public static bool IsLoggerAvailable()
        {
            return ProviderIsAvailableOverride && GetLogManagerType() != null;
        }

        private static Type GetLogManagerType()
        {
            return Type.GetType("log4net.LogManager, log4net");
        }

        private static Func<string, object> GetGetLoggerMethodCall()
        {
            var logManagerType = GetLogManagerType();
            var method = logManagerType.GetMethod("GetLogger", new[] {typeof(string)});
            ParameterExpression resultValue;
            var keyParam = Expression.Parameter(typeof(string), "key");
            var methodCall = Expression.Call(null, method, resultValue = keyParam);
            return Expression.Lambda<Func<string, object>>(methodCall, resultValue).Compile();
        }

        public class Log4NetLogger : ILog
        {
            private static readonly Type LoggerType = Type.GetType("log4net.ILog, log4net");

            private static readonly Func<object, bool> IsDebugEnabledDelegate;
            private static readonly Action<object, string> DebugDelegate;
            private static readonly Action<object, string, Exception> DebugExceptionDelegate;

            private static readonly Func<object, bool> IsInfoEnabledDelegate;
            private static readonly Action<object, string> InfoDelegate;
            private static readonly Action<object, string, Exception> InfoExceptionDelegate;

            private static readonly Func<object, bool> IsWarnEnabledDelegate;
            private static readonly Action<object, string> WarnDelegate;
            private static readonly Action<object, string, Exception> WarnExceptionDelegate;

            private static readonly Func<object, bool> IsErrorEnabledDelegate;
            private static readonly Action<object, string> ErrorDelegate;
            private static readonly Action<object, string, Exception> ErrorExceptionDelegate;

            private static readonly Func<object, bool> IsFatalEnabledDelegate;
            private static readonly Action<object, string> FatalDelegate;
            private static readonly Action<object, string, Exception> FatalExceptionDelegate;
            private readonly object _logger;

            static Log4NetLogger()
            {
                IsDebugEnabledDelegate = GetPropertyGetter("IsDebugEnabled");
                DebugDelegate = GetMethodCallForMessage("Debug");
                DebugExceptionDelegate = GetMethodCallForMessageException("Debug");

                IsInfoEnabledDelegate = GetPropertyGetter("IsInfoEnabled");
                InfoDelegate = GetMethodCallForMessage("Info");
                InfoExceptionDelegate = GetMethodCallForMessageException("Info");

                IsErrorEnabledDelegate = GetPropertyGetter("IsErrorEnabled");
                ErrorDelegate = GetMethodCallForMessage("Error");
                ErrorExceptionDelegate = GetMethodCallForMessageException("Error");

                IsWarnEnabledDelegate = GetPropertyGetter("IsWarnEnabled");
                WarnDelegate = GetMethodCallForMessage("Warn");
                WarnExceptionDelegate = GetMethodCallForMessageException("Warn");

                IsFatalEnabledDelegate = GetPropertyGetter("IsFatalEnabled");
                FatalDelegate = GetMethodCallForMessage("Fatal");
                FatalExceptionDelegate = GetMethodCallForMessageException("Fatal");
            }

            public Log4NetLogger(object logger)
            {
                _logger = logger;
            }

            public void Log(LogLevel logLevel, Func<string> messageFunc)
            {
                switch (logLevel)
                {
                    case LogLevel.Info:
                        if (IsInfoEnabledDelegate(_logger)) InfoDelegate(_logger, messageFunc());
                        break;
                    case LogLevel.Warn:
                        if (IsWarnEnabledDelegate(_logger)) WarnDelegate(_logger, messageFunc());
                        break;
                    case LogLevel.Error:
                        if (IsErrorEnabledDelegate(_logger)) ErrorDelegate(_logger, messageFunc());
                        break;
                    case LogLevel.Fatal:
                        if (IsFatalEnabledDelegate(_logger)) FatalDelegate(_logger, messageFunc());
                        break;
                    default:
                        if (IsDebugEnabledDelegate(_logger)) DebugDelegate(_logger, messageFunc());
                        break;
                }
            }

            public void Log<TException>(LogLevel logLevel, Func<string> messageFunc, TException exception)
                where TException : Exception
            {
                switch (logLevel)
                {
                    case LogLevel.Info:
                        if (IsInfoEnabledDelegate(_logger)) InfoExceptionDelegate(_logger, messageFunc(), exception);
                        break;
                    case LogLevel.Warn:
                        if (IsWarnEnabledDelegate(_logger)) WarnExceptionDelegate(_logger, messageFunc(), exception);
                        break;
                    case LogLevel.Error:
                        if (IsErrorEnabledDelegate(_logger)) ErrorExceptionDelegate(_logger, messageFunc(), exception);
                        break;
                    case LogLevel.Fatal:
                        if (IsFatalEnabledDelegate(_logger)) FatalExceptionDelegate(_logger, messageFunc(), exception);
                        break;
                    default:
                        if (IsDebugEnabledDelegate(_logger)) DebugExceptionDelegate(_logger, messageFunc(), exception);
                        break;
                }
            }

            private static Func<object, bool> GetPropertyGetter(string propertyName)
            {
                var funcParam = Expression.Parameter(typeof(object), "l");
                Expression convertedParam = Expression.Convert(funcParam, LoggerType);
                Expression property = Expression.Property(convertedParam, propertyName);
                return (Func<object, bool>) Expression.Lambda(property, funcParam).Compile();
            }

            private static Action<object, string> GetMethodCallForMessage(string methodName)
            {
                var loggerParam = Expression.Parameter(typeof(object), "l");
                var messageParam = Expression.Parameter(typeof(string), "o");
                Expression convertedParam = Expression.Convert(loggerParam, LoggerType);
                var method = LoggerType.GetMethod(methodName, new[] {typeof(string)});
                var methodCall = Expression.Call(convertedParam, method, messageParam);
                return (Action<object, string>) Expression.Lambda(methodCall, loggerParam, messageParam).Compile();
            }

            private static Action<object, string, Exception> GetMethodCallForMessageException(string methodName)
            {
                var loggerParam = Expression.Parameter(typeof(object), "l");
                var messageParam = Expression.Parameter(typeof(string), "o");
                var exceptionParam = Expression.Parameter(typeof(Exception), "e");
                Expression convertedParam = Expression.Convert(loggerParam, LoggerType);
                var method = LoggerType.GetMethod(methodName, new[] {typeof(string), typeof(Exception)});
                var methodCall = Expression.Call(convertedParam, method, messageParam, exceptionParam);
                return (Action<object, string, Exception>) Expression
                    .Lambda(methodCall, loggerParam, messageParam, exceptionParam).Compile();
            }
        }
    }
}