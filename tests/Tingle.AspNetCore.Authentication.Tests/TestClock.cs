﻿using Microsoft.AspNetCore.Authentication;

namespace Tingle.AspNetCore.Authentication.Tests;

public class TestClock : ISystemClock
{
    public TestClock()
    {
        UtcNow = new DateTimeOffset(2013, 6, 11, 12, 34, 56, 789, TimeSpan.Zero);
    }

    public DateTimeOffset UtcNow { get; set; }

    public void Add(TimeSpan timeSpan)
    {
        UtcNow = UtcNow + timeSpan;
    }
}
