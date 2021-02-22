using Version = System.Version;
namespace Qurre
{
	public abstract class Plugin
	{
		public static YamlConfig Config;
		public virtual string Developer { get; } = "";
		public virtual string Name { get; } = "";
		public virtual Version Version { get; } = new Version(1, 0, 0);
		public virtual Version NeededQurreVersion { get; } = new Version(1, 1, 1);
		public abstract void Enable();
		public abstract void Disable();
		public abstract void Reload();
	}
}