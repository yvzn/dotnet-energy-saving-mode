# Energy Saving Mode for ASP.NET

Automatically pause parts of application outside business hours
and optimise for cloud costs (FinOps) and efficiency (GreenOps)

## Description

An option to reduce cloud costs or carbon footprint of an application, is to reduce
its energy usage outside operating hours. For instance: reduce compute and storage
requirements at night and during weekends.

One way to achieve this, is to shut down the entire application and underlying services
when they are not in use.

But this is not always feasible.

_Energy Saving Mode for ASP.NET_ proposes to programmatically
pause selected services or components on a desired schedule:

1. define a weekly schedule of when Energy Saving Mode should be enabled
2. react accordingly when Energy Saving Mode changes
3. check the Energy Saving Mode status before running intensive tasks

## Example scenarios

When Energy Saving Mode is enabled:

1. Reduce log levels and telemetry levels 
1. Reduce or turn off health checks
1. Close all SignalR connections and clients
1. Flush internal caches
1. ...

See the provided samples in the [sample](https://github.com/yvzn/dotnet-energy-saving-mode/tree/main/sample) folder

## Installation

Requires ASP.NET 8+

```bash
dotnet add package EnergySavingMode --prerelease
```

### Configuration

Define a weekly schedule of when Energy Saving Mode is enabled:

The time ranges are expressed in the **local timezone of the server**.
Extra precautions are required when server is operating outside the usual timezone
(for instance in another cloud region)

```jsonc
// appsettings.json
{
    // ...
    "EnergySavingMode" : {
        "Enabled": {
            "Mon" : ["00:00-07:59", "20:00-23:59"],
            "Tue" : [ /* ... */ ],
            // ...
            "Sun" : [ /* ... */ ]
        }
    }
}
```

Outside of this schedule, Energy Saving Mode is disabled.

### Register the EnergySavingMode service

```csharp
// Program.cs
builder.Services.AddEnergySavingMode(
	builder.Configuration.GetSection("EnergySavingMode"));
```

### Usage

Listen to events and react accordingly when Energy Saving Mode changes:

```csharp
public class MyComponent(IEnergySavingModeEvents energySavingMode) {
	// ...

	public void MyStartupCode() {
		energySavingMode.OnEnabled(EnergySavingMode_Enabled);
		energySavingMode.OnDisabled(EnergySavingMode_Disabled);
	}
	
	private Task EnergySavingMode_Enabled()
	{
		// ...
	}

	private Task EnergySavingMode_Disabled()
	{
		// ...
	}
}
```

Check the Energy Saving Mode status before running intensive tasks:

```csharp
public class MyComponent(IEnergySavingModeStatus energySavingModeStatus) {
	// ...

	public void MyIntensiveComputation() {
		if (energySavingModeStatus.Current.IsEnabled) {
			// delay until later
			return;
		}

		// ...
	}
}
```

## License

[Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)

Copyright 2024 Yvan Razafindramanana

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
