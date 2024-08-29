# Energy Saving Mode for ASP.NET

Automatically pause parts of application outside business hours
and optimise for cloud costs (FinOps) and efficiency (GreenOps)

## Description

To reduce cloud costs or carbon footprint of an application, an option is to reduce
its energy usage outside operating hours. For instance: reduce compute and storage
requirements at night and during weekends.

The simplest way to achieve this goal is to shut down entire applications and
underlying services.

This is not always feasible.

The alternative proposed by _Energy Saving Mode for ASP.NET_ is to programmatically
pause the most impactful services or components on a desired schedule:

1. define a weekly schedule of when application should be in Energy Saving Mode
2. listen to events when Energy Saving Mode is enabled or disabled — and act act accordingly
3. or check the Energy Saving Mode status before running intensive tasks

More example scenarios described below.

## Installation

Requires ASP.NET 8+

```bash
dotnet add package EnergySavingMode --prerelease
```

### Configuration

Define a weekly schedule of when application should be in Energy Saving Mode.

The time ranges are expressed in the **local timezone of the server**.

Extra precautions required when server is operating outside the usual timezone
(for instance in another cloud region)

```jsonc
// appsettings.json
{
    // ...
    "EnergySavingMode" : {
        "Enabled": {
            "Mon" : [00:00-07:59, 20:00-23:59],
            "Tue" : [ /* ... */ ],
            // ...
            "Sun" : [ /* ... */ ]
        }
    }
}
```

### Registering the EnergySavingMode service

```csharp
// Program.cs
builder.Services.AddEnergySavingMode(
	builder.Configuration.GetSection("EnergySavingMode"));
```

### Usage

Listen to events when Energy Saving Mode is enabled or disabled — and act act accordingly

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

Check the Energy Saving Mode status before running intensive tasks

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

## Example scenarios

1. Reduce log levels and telemetry levels when Energy Saving Mode is enabled, and restore them when Energy Saving Mode is disabled
1. Reduce or turn off health checks
1. Close all SignalR connections and clients — and configure automatic reconnect
1. Flush internal caches
1. ...

See also the provided samples in the [sample](https://github.com/yvzn/dotnet-energy-saving-mode/tree/main/sample) folder

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
