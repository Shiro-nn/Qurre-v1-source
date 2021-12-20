namespace Qurre.API.Addons
{
    /// <summary>
    /// The main configuration variables.
    /// </summary>
    public interface IYamlConfig
    {
        /// <summary>
        /// Enable the plugin?
        /// </summary>
        bool IsEnabled { get; set; }
    }
}