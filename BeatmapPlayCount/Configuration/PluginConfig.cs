using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config;
using IPA.Config.Data;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatmapPlayCount.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        // Members must be 'virtual' if we want BSIPA to detect a value change and save the config automatically.

        public virtual float MinimumSongProgressToIncrementingPlayCount { get; set; } = 0.7f;

        public virtual bool IncrementCountInPracticeMode { get; set; } = true;

        [NonNullable, UseConverter(typeof(CollectionConverter<string, HashSet<string>>))]
        public virtual HashSet<string> BannedBeatmapCharacteristics { get; set; } = new HashSet<string> { "Lightshow" };
    }
}
