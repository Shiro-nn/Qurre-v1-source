using Qurre.API;
using Version = System.Version;
namespace Qurre
{
	public abstract class Plugin
	{
		public static Config Config { get; set; }
		public virtual string Developer { get; } = "";
		public virtual string Name { get; } = "";
		public virtual Version Version { get; } = new Version(1, 0, 0);
		public virtual Version NeededQurreVersion { get; } = new Version(1, 4, 0);
		public virtual int Priority { get; } = 0;
		public abstract void Enable();
		public abstract void Disable();
		public virtual void Reload() => Log.Debug($"Reloaded.\nPlugin - {Name}\nDeveloper - {Developer}\nVersion - {Version}\nNeeded Qurre Version - {NeededQurreVersion}");
	}
}