using Version = System.Version;
namespace Qurre
{
	public abstract class Plugin
	{
		public static YamlConfig Config;
		public virtual string Developer { get; } = "";
		public virtual string Name { get; } = "";
		public virtual Version Version { get; } = new Version(1, 0, 0);
		public virtual Version NeededQurreVersion { get; } = new Version(1, 1, 2, 1);
		public virtual void Enable() => Log.Debug($"Enabled.\nPlugin - {Name}\nDeveloper - {Developer}\nVersion - {Version}\nNeeded Qurre Version - {NeededQurreVersion}");
		public virtual void Disable() => Log.Debug($"Disabled.\nPlugin - {Name}\nDeveloper - {Developer}\nVersion - {Version}\nNeeded Qurre Version - {NeededQurreVersion}");
		public virtual void Reload() => Log.Debug($"Reloaded.\nPlugin - {Name}\nDeveloper - {Developer}\nVersion - {Version}\nNeeded Qurre Version - {NeededQurreVersion}");
	}
}