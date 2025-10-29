# Bug Fix: Quality Selection Infinite Loop

## Issue
After selecting a quality profile from the dropdown, the system would display the quality selection screen again instead of showing the request button, causing an infinite loop.

## Root Cause
The quality selection interactions were never being routed to their handlers in `ChatBot.cs`.

### TV Shows (Line 393)
```csharp
// BEFORE (BROKEN):
else if (e.Id.ToLower().StartsWith("tr") || e.Id.ToLower().StartsWith("ts"))
{
    await HandleTvRequestAsync(e);
}

// Problem: "TQS" starts with "tq", not "tr" or "ts", so HandleTvRequestAsync was never called!
```

### Movies (Line 380)
```csharp
// BEFORE (BROKEN):
if (e.Id.ToLower().StartsWith("mr"))
{
    await HandleMovieRequestAsync(e);
}

// Problem: "MQS" starts with "mq", not "mr", so HandleMovieRequestAsync was never called!
```

## Solution
Updated the routing conditions in `ChatBot.cs` to include the quality selection prefixes:

### TV Shows Fix
```csharp
// AFTER (FIXED):
else if (e.Id.ToLower().StartsWith("tr") || e.Id.ToLower().StartsWith("ts") || e.Id.ToLower().StartsWith("tq"))
{
    await HandleTvRequestAsync(e);
}
```

### Movies Fix
```csharp
// AFTER (FIXED):
if (e.Id.ToLower().StartsWith("mr") || e.Id.ToLower().StartsWith("mq"))
{
    await HandleMovieRequestAsync(e);
}
```

## Result
When users select a quality profile from either TQS (TV) or MQS (Movie) dropdowns:
1. The interaction is now properly routed to the respective handler
2. The quality profile selection is processed
3. The request button appears as expected
4. No more infinite loop!

## Files Changed
- `Requestrr.WebApi/RequestrrBot/ChatBot.cs` (Lines 380 and 393)

## Build Location
The fixed version has been published to:
`requestrr-win-x64-quality_selection_FIXED/`

This is a drop-in replacement - just stop your current instance, replace the folder, and start it again.
Your `config.json` will be preserved.
