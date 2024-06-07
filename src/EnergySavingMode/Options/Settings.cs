namespace EnergySavingMode.Options;

internal class Settings
{
	public Schedule Enabled { get; set; } = new();
}

internal class Schedule
{
	public ICollection<string> Mon { get; set; } = [];
	public ICollection<string> Tue { get; set; } = [];
	public ICollection<string> Wed { get; set; } = [];
	public ICollection<string> Thu { get; set; } = [];
	public ICollection<string> Fri { get; set; } = [];
	public ICollection<string> Sat { get; set; } = [];
	public ICollection<string> Sun { get; set; } = [];
}
