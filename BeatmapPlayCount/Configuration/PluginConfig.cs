using System.Runtime.CompilerServices;
using IPA.Config;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatmapPlayCount.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        // Must be 'virtual' if you want BSIPA to detect a value change and save the config automatically.
        public virtual float MinimumSongProgressToIncrementingPlayCount { get; set; } = 0.7f;
    }
}
