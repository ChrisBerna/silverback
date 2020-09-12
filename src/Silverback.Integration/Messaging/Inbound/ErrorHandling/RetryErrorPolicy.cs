﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Silverback.Diagnostics;
using Silverback.Messaging.Messages;

namespace Silverback.Messaging.Inbound.ErrorHandling
{
    /// <summary>
    ///     This policy retries the handler method multiple times in case of exception. An optional delay can be
    ///     specified.
    /// </summary>
    /// TODO: Exponential backoff variant
    public class RetryErrorPolicy : ErrorPolicyBase
    {
        private readonly TimeSpan _initialDelay;

        private readonly TimeSpan _delayIncrement;

        private readonly ISilverbackIntegrationLogger _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryErrorPolicy" /> class.
        /// </summary>
        /// <param name="serviceProvider">
        ///     The <see cref="IServiceProvider" />.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ISilverbackIntegrationLogger" />.
        /// </param>
        /// <param name="initialDelay">
        ///     The optional delay to be applied to the first retry.
        /// </param>
        /// <param name="delayIncrement">
        ///     The optional increment to the delay to be applied at each retry.
        /// </param>
        public RetryErrorPolicy(
            IServiceProvider serviceProvider,
            ISilverbackIntegrationLogger<RetryErrorPolicy> logger,
            TimeSpan? initialDelay = null,
            TimeSpan? delayIncrement = null)
            : base(serviceProvider, logger)
        {
            _initialDelay = initialDelay ?? TimeSpan.Zero;
            _delayIncrement = delayIncrement ?? TimeSpan.Zero;
            _logger = logger;
        }

        /// <inheritdoc cref="ErrorPolicyBase.ApplyPolicy" />
        protected override async Task<ErrorAction> ApplyPolicy(
            IReadOnlyCollection<IRawInboundEnvelope> envelopes,
            Exception exception)
        {
            await ApplyDelay(envelopes).ConfigureAwait(false);

            _logger.LogInformationWithMessageInfo(
                IntegrationEventIds.RetryMessageProcessing,
                "The message(s) will be processed again.",
                envelopes);

            return ErrorAction.Retry;
        }

        private async Task ApplyDelay(IReadOnlyCollection<IRawInboundEnvelope> envelopes)
        {
            var delay = (int)_initialDelay.TotalMilliseconds +
                        (envelopes.First().Headers.GetValueOrDefault<int>(DefaultMessageHeaders.FailedAttempts) *
                         (int)_delayIncrement.TotalMilliseconds);

            if (delay <= 0)
                return;

            _logger.LogTraceWithMessageInfo(
                IntegrationEventIds.RetryDelayed,
                $"Waiting {delay} milliseconds before retrying to process the message(s).",
                envelopes);

            await Task.Delay(delay).ConfigureAwait(false);
        }
    }
}
