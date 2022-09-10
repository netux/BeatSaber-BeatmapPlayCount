using BeatmapPlayCount.Configuration;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using Zenject;

namespace BeatmapPlayCount.Views
{
    [HotReload(RelativePathToLayout = @"SettingViewController.bsml")]
    internal class SettingViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [UIValue("MinimumSongProgressToIncrementingPlayCount")]
        public float MinimumSongProgressToIncrementingPlayCount
        {
            get => PluginConfig.Instance.MinimumSongProgressToIncrementingPlayCount * 100.0f;
            set => PluginConfig.Instance.MinimumSongProgressToIncrementingPlayCount = value / 100.0f;
        }

        public string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        [Inject]
        public void Initialize()
        {
            BSMLSettings.instance.AddSettingsMenu("Play Count", ResourceName, this);
        }

        public void Dispose()
        {
            if (BSMLSettings.instance != null)
            {
                BSMLSettings.instance.RemoveSettingsMenu(this);
            }
        }
    }
}
