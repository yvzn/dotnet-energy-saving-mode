namespace EnergySavingMode.Services;

internal static class TimeOnlyExtensions
{
	public static bool IsBefore(this TimeOnly t1, TimeOnly t2)
	{
		//                      grace period    t2
		// -----------------|*******************|---->
		// <===[is before]  |    [is after]=======>

		var gracePeriod = TimeSpan.FromMilliseconds(10);
		var isReallyBefore = t1.Add(gracePeriod) < t2;

		return isReallyBefore;
	}
}
