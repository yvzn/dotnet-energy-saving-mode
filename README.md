# Energy Saving Mode for ASP.NET

## Configuration

```jsonc
// appsettings.json
{
    // ...
    "EnergySavingMode" : {
        "Enabled": {
            "Mon" : [00:00-07:59, 20:00-23:59]
            "Tue" : [ /* ... */ ],
            // ...
            "Sun" : [ /* ... */ ]
        }
    }
}
```


## Background Service

```csharp
// Program.cs
builder.Services.AddEnergySavingMode(
	builder.Configuration.GetSection("EnergySavingMode"));
```

## Usage

```csharp

this.EnergySavingMode.Enabled += MyEventHandler

this.EnergySavingMode.Disabled += MyEventHandler

```

## Possible Implementation

Detect the next event in the calendar

Make sure the next next event is not immediatly contiguous

Use timer to trigger event handlers

Repeat
