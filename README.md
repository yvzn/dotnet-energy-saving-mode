# Energy Saving Mode for ASP.NET

TODO Description

## Installation

TODO

## Configuration

The time ranges and dates are expressed in the **local timezone of the server**.

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

## Registering the EnergySavingMode service

```csharp
// Program.cs
builder.Services.AddEnergySavingMode(
	builder.Configuration.GetSection("EnergySavingMode"));
```

## Usage

```csharp
public class MyComponent(IEnergySavingModeEvents energySavingMode) {
	// ...

	energySavingMode.Enabled += MyEventHandler;
	energySavingMode.Disabled += MyEventHandler;

	// ...
}
```
