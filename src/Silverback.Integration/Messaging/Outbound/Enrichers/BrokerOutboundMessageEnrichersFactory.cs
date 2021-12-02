﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Silverback.Messaging.Messages;

namespace Silverback.Messaging.Outbound.Enrichers
{
    internal sealed class BrokerOutboundMessageEnrichersFactory : IBrokerOutboundMessageEnrichersFactory
    {
        private static readonly NullEnricher NullEnricherInstance = new();

        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<Type, Type> _enricherTypeCache = new();

        public BrokerOutboundMessageEnrichersFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IMovePolicyMessageEnricher> GetMovePolicyEnrichers(IEndpoint endpoint)
        {
            var enricherType = _enricherTypeCache.GetOrAdd(
                endpoint.GetType(),
                type => typeof(IMovePolicyMessageEnricher<>)
                    .MakeGenericType(type));

            var headersEnricher = (IMovePolicyMessageEnricher?)_serviceProvider.GetService(enricherType);

            yield return headersEnricher ?? NullEnricherInstance;
        }

        private sealed class NullEnricher : IMovePolicyMessageEnricher
        {
            public void Enrich(IRawInboundEnvelope inboundEnvelope, IOutboundEnvelope outboundEnvelope, Exception exception)
            {
            }
        }
    }
}
