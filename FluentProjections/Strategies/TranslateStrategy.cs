using System;
using System.Collections.Generic;
using System.Linq;
using FluentProjections.Logging;
using FluentProjections.Logging.Generic;
using FluentProjections.Persistence;

namespace FluentProjections.Strategies
{
    public class TranslateStrategy<TMessage, TTranslatedMessage> : IMessageHandlingStrategy<TMessage>
    {
        private static readonly ILog<TMessage, TTranslatedMessage> Logger =
            LogProvider<TMessage, TTranslatedMessage>.GetLogger(typeof (TranslateStrategy<TMessage, TTranslatedMessage>));

        private readonly IMessageHandlingStrategy<TTranslatedMessage> _strategy;
        private readonly Func<TMessage, IEnumerable<TTranslatedMessage>> _translate;

        public TranslateStrategy(
            Func<TMessage, IEnumerable<TTranslatedMessage>> translate,
            IMessageHandlingStrategy<TTranslatedMessage> strategy
            )
        {
            _translate = translate;
            _strategy = strategy;
        }

        public virtual void Handle(TMessage message, IProvideProjections store)
        {
            var translatedMessages = Translate(message);
            foreach (var translatedMessage in translatedMessages)
            {
                Handle(translatedMessage, store);
            }
        }

        private IEnumerable<TTranslatedMessage> Translate(TMessage message)
        {
            Logger.DebugFormat("Translate a message: {0}", message);
            try
            {
                return _translate(message).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to translate a message.", e);
                throw;
            }
        }

        private void Handle(TTranslatedMessage message, IProvideProjections store)
        {
            Logger.DebugFormat("Handle a translated message: {0}", message);
            try
            {
                _strategy.Handle(message, store);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Failed to handle translated message.", e);
                throw;
            }
        }
    }
}