﻿using Orangotango.Core.Events;
using System;

namespace Orangotango.Events.Rooms.Category;

public class CategoryCreatedUpdatedEvent : Event
{
    public string Name { get; private set; }

    public CategoryCreatedUpdatedEvent()
    {
    }

    public CategoryCreatedUpdatedEvent(Guid aggregateId,
        string name)
    {
        AggregateId = aggregateId;
        Name = name;
    }
}