using BeatmapPlayCount.Configuration;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace BeatmapPlayCount.Views
{
    [HotReload(RelativePathToLayout = @"SettingViewController.bsml")]
    internal class SettingViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [Inject]
        private readonly BeatmapCharacteristicCollectionSO baseGameBeatmapCharacteristicCollection = null!;

        [UIValue("MinimumSongProgressToIncrementingPlayCount")]
        public float MinimumSongProgressToIncrementingPlayCount
        {
            get => PluginConfig.Instance.MinimumSongProgressToIncrementingPlayCount * 100.0f;
            set => PluginConfig.Instance.MinimumSongProgressToIncrementingPlayCount = value / 100.0f;
        }

        [UIValue("IncrementCountInPracticeMode")]
        public bool IncrementCountInPracticeMode
        {
            get => PluginConfig.Instance.IncrementCountInPracticeMode;
            set => PluginConfig.Instance.IncrementCountInPracticeMode = value;
        }

        [UIValue("BannedBeatmapCharacteristics")]
        public readonly List<BeatmapCharacteristicBannedToggle> BannedBeatmapCharacteristics = new List<BeatmapCharacteristicBannedToggle>();

        public string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        public void Initialize()
        {
            var allBeatmapCharacteristics = new List<BeatmapCharacteristicSO>();
            allBeatmapCharacteristics.AddRange(baseGameBeatmapCharacteristicCollection.beatmapCharacteristics);
            allBeatmapCharacteristics.AddRange(SongCore.Collections.customCharacteristics);
            PopulateBannedBeatmapCharacteristics(
                allBeatmapCharacteristics,
                PluginConfig.Instance.BannedBeatmapCharacteristics.ToArray()
            );

            BSMLSettings.instance.AddSettingsMenu("Play Count", ResourceName, this);
        }

        public void Dispose()
        {
            if (BSMLSettings.instance != null)
            {
                BSMLSettings.instance.RemoveSettingsMenu(this);
            }
        }

        public void PopulateBannedBeatmapCharacteristics(
            List<BeatmapCharacteristicSO> beatmapCharacteristics,
            string[] bannedBeatmapCharacteristicsNames)
        {
#if DEBUG
            Plugin.Log.Debug($"Populating {beatmapCharacteristics.Count} beatmap characteristics for the settings view");
#endif

            BannedBeatmapCharacteristics.Clear();
            foreach (var beatmapCharacteristic in beatmapCharacteristics)
            {
                BannedBeatmapCharacteristics.Add(
                    new BeatmapCharacteristicBannedToggle(this)
                    {
                        Characteristic = beatmapCharacteristic,
                        IsBanned = bannedBeatmapCharacteristicsNames
                            .Any(serializedName => serializedName == beatmapCharacteristic.serializedName)
                    }
                );
            }
        }

        public void HandleBannedBeatmapCharacteristicsChange()
        {
            var newValue = new HashSet<string>();
            foreach (var beatmapCharacteristicsBannedToggle in BannedBeatmapCharacteristics)
            {
                if (beatmapCharacteristicsBannedToggle.IsBanned)
                {
                    newValue.Add(beatmapCharacteristicsBannedToggle.Characteristic.serializedName);
                }
            }
            PluginConfig.Instance.BannedBeatmapCharacteristics = newValue;
        }

        internal class BeatmapCharacteristicBannedToggle
        {
            private readonly SettingViewController settingViewController;

            internal BeatmapCharacteristicBannedToggle(SettingViewController _settingViewController)
            {
                settingViewController = _settingViewController;
            }

            public BeatmapCharacteristicSO Characteristic;
            public bool IsBanned;

            public string LocalizedCharacteristicName
            {
                get
                {
                    return Localization.Get(Characteristic.characteristicNameLocalizationKey);
                }
            }

            [UIAction("HandleChange")]
            public void HandleChange(bool value)
            {
                IsBanned = value;

                settingViewController.HandleBannedBeatmapCharacteristicsChange();
            }
        }
    }
}
