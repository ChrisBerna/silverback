﻿using System;
using Silverback.Messaging.Messages;

namespace Silverback.Messaging
{
    /// <summary>
    /// The base class for all subscribers.
    /// </summary>
    public abstract class Subscriber : IDisposable
    {
        private readonly IDisposable _subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriber" /> class.
        /// </summary>
        /// <param name="messages">The observable stream of messages.</param>
        /// <exception cref="System.ArgumentNullException">messages</exception>
        protected Subscriber(IObservable<IMessage> messages)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));

            // TODO: Must handle OnError and OnComplete?
            _subscription = messages.Subscribe(OnNext);
        }

        /// <summary>
        /// Called when a message is published.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void OnNext(IMessage message);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _subscription?.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Subscriber"/> class.
        /// </summary>
        ~Subscriber()
        {
            Dispose(false);
        }
    }
}