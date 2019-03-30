﻿// Copyright (c) 2018-2019 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using Silverback.Domain;

namespace Silverback.Tests.Core.Model.TestTypes.Domain
{
    public class TestAggregateRoot : DomainEntity, IAggregateRoot
    {
        public new void AddEvent(IDomainEvent<IDomainEntity> domainEvent)
            => base.AddEvent(domainEvent);

        public new TEvent AddEvent<TEvent>() 
            where TEvent : IDomainEvent<IDomainEntity>, new()
            => base.AddEvent<TEvent>();
    }
}