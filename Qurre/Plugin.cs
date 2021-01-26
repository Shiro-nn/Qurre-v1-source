namespace Qurre
{
	public abstract class Plugin
    {
        public static YamlConfig Config;
        public abstract string Name { get; }
		public abstract string Version { get; }
		public abstract string Developer { get; }
		public abstract string Owner { get; }
		public abstract void Enable();
		public abstract void Disable();
		public abstract void Reload();
	}
}