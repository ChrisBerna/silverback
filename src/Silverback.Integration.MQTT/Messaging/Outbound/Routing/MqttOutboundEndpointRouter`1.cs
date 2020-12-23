﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using System.Linq;
using Silverback.Messaging.Configuration.Mqtt;
using Silverback.Messaging.Messages;
using Silverback.Util;

namespace Silverback.Messaging.Outbound.Routing
{
    /// <summary>
    ///     Routes the outbound messages to one or multiple MQTT endpoints.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The type of the messages to be routed.
    /// </typeparam>
    public class MqttOutboundEndpointRouter<TMessage> : OutboundRouter<TMessage>
    {
        private readonly Dictionary<string, MqttProducerEndpoint> _endpoints;

        private readonly RouterFunction _routerFunction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MqttOutboundEndpointRouter{TMessage}" /> class.
        /// </summary>
        /// <param name="routerFunction">
        ///     The <see cref="SingleEndpointRouterFunction" />.
        /// </param>
        /// <param name="endpointBuilderActions">
        ///     The <see cref="IReadOnlyDictionary{TKey,TValue}" /> containing the key of each endpoint and the
        ///     <see cref="Action{T}" /> to be invoked to build them.
        /// </param>
        /// <param name="clientConfig">
        ///     The <see cref="MqttClientConfig" />.
        /// </param>
        public MqttOutboundEndpointRouter(
            SingleEndpointRouterFunction routerFunction,
            IReadOnlyDictionary<string, Action<IMqttProducerEndpointBuilder>> endpointBuilderActions,
            MqttClientConfig clientConfig)
            : this(
                (message, headers, endpoints) => new[] { routerFunction.Invoke(message, headers, endpoints) },
                endpointBuilderActions,
                clientConfig)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MqttOutboundEndpointRouter{TMessage}" /> class.
        /// </summary>
        /// <param name="routerFunction">
        ///     The <see cref="RouterFunction" />.
        /// </param>
        /// <param name="endpointBuilderActions">
        ///     The <see cref="IReadOnlyDictionary{TKey,TValue}" /> containing the key of each endpoint and the
        ///     <see cref="Action{T}" /> to be invoked to build them.
        /// </param>
        /// <param name="clientConfig">
        ///     The <see cref="MqttClientConfig" />.
        /// </param>
        public MqttOutboundEndpointRouter(
            RouterFunction routerFunction,
            IReadOnlyDictionary<string, Action<IMqttProducerEndpointBuilder>> endpointBuilderActions,
            MqttClientConfig clientConfig)
        {
            _routerFunction = Check.NotNull(routerFunction, nameof(routerFunction));
            _endpoints = Check.NotNull(endpointBuilderActions, nameof(endpointBuilderActions))
                .ToDictionary(
                    pair => pair.Key,
                    pair => BuildEndpoint(pair.Value, clientConfig));
        }

        /// <summary>
        ///     The actual router method that receives the message (including its headers) together with the
        ///     dictionary containing all endpoints and returns the destination endpoints.
        /// </summary>
        /// <param name="message">
        ///     The message to be routed.
        /// </param>
        /// <param name="headers">
        ///     The message headers.
        /// </param>
        /// <param name="endpoints">
        ///     The dictionary containing all configured endpoints for this router.
        /// </param>
        /// <returns>
        ///     The destination endpoints.
        /// </returns>
        public delegate IEnumerable<MqttProducerEndpoint> RouterFunction(
            TMessage message,
            MessageHeaderCollection headers,
            IReadOnlyDictionary<string, MqttProducerEndpoint> endpoints);

        /// <summary>
        ///     The actual router method that receives the message (including its headers) together with the
        ///     dictionary containing all endpoints and returns the destination endpoint.
        /// </summary>
        /// <param name="message">
        ///     The message to be routed.
        /// </param>
        /// <param name="headers">
        ///     The message headers.
        /// </param>
        /// <param name="endpoints">
        ///     The dictionary containing all configured endpoints for this router.
        /// </param>
        /// <returns>
        ///     The destination endpoint.
        /// </returns>
        public delegate MqttProducerEndpoint SingleEndpointRouterFunction(
            TMessage message,
            MessageHeaderCollection headers,
            IReadOnlyDictionary<string, MqttProducerEndpoint> endpoints);

        /// <inheritdoc cref="IOutboundRouter.Endpoints" />
        public override IEnumerable<IProducerEndpoint> Endpoints => _endpoints.Values;

        /// <inheritdoc cref="IOutboundRouter{TMessage}.GetDestinationEndpoints(TMessage,MessageHeaderCollection)" />
        public override IEnumerable<IProducerEndpoint> GetDestinationEndpoints(
            TMessage message,
            MessageHeaderCollection headers) =>
            _routerFunction.Invoke(message, headers, _endpoints);

        private static MqttProducerEndpoint BuildEndpoint(
            Action<IMqttProducerEndpointBuilder> builderAction,
            MqttClientConfig clientConfig)
        {
            var builder = new MqttProducerEndpointBuilder(clientConfig);

            builderAction.Invoke(builder);

            return builder.Build();
        }
    }
}
