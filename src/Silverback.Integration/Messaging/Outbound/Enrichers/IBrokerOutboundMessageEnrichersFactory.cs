﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System.Collections.Generic;

namespace Silverback.Messaging.Outbound.Enrichers
{
    /// <summary>
    ///     Provides the list of <see cref="IMovePolicyMessageEnricher" /> according to the specified endpoint.
    /// </summary>
    public interface IBrokerOutboundMessageEnrichersFactory
    {
        /// <summary>
        ///     Returns the list of <see cref="IMovePolicyMessageEnricher" /> for the specified endpoint.
        /// </summary>
        /// <param name="endpoint">
        ///     The endpoint.
        /// </param>
        /// <returns>
        ///     The list of <see cref="IMovePolicyMessageEnricher" /> that matches the specified endpoint type.
        /// </returns>
        IEnumerable<IMovePolicyMessageEnricher> GetMovePolicyEnrichers(IEndpoint endpoint);
    }
}
