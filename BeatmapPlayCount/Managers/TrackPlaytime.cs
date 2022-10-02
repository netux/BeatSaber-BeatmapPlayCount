using BeatmapPlayCount.Configuration;
using System.Linq;
using Zenject;

namespace BeatmapPlayCount.Managers
{
    internal class TrackPlaytime : IInitializable, ITickable
    {
        private readonly AudioTimeSyncController audioSyncController;
        private readonly string beatmapId;
        private readonly string beatmapCharacteristic;

        public bool Incremented { get; private set; }
        public bool IsGameplayAnExternalModReplay { get; private set; }
        public bool DoesBeatmapHaveBannedCharacteristic { get; private set; }

        public TrackPlaytime(IDifficultyBeatmap _currentlyPlayingLevel, AudioTimeSyncController _audioSyncController)
        {
            audioSyncController = _audioSyncController;
            beatmapId = _currentlyPlayingLevel.level.levelID;
            beatmapCharacteristic = _currentlyPlayingLevel.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
        }

        public void Initialize()
        {
            Incremented = false;
            IsGameplayAnExternalModReplay = Utils.ExternalReplayMod.CheckIfGameplayIsAReplay();
            DoesBeatmapHaveBannedCharacteristic = PluginConfig.Instance.BannedBeatmapCharacteristics
                .Any(bannedCharacteristic => bannedCharacteristic == beatmapCharacteristic);
#if DEBUG
            Plugin.Log.Info($"TrackPlaytime IsGameplayAnExternalModReplay = {IsGameplayAnExternalModReplay}; DoesBeatmapHaveBannedCharacteristic = {DoesBeatmapHaveBannedCharacteristic}");
#endif
        }

        public void Tick()
        {
            if (IsGameplayAnExternalModReplay || DoesBeatmapHaveBannedCharacteristic || Incremented)
            {
                return;
            }

            var progress = audioSyncController.songTime / audioSyncController.songEndTime;
            if (progress >= PluginConfig.Instance.MinimumSongProgressToIncrementingPlayCount)
            {
                Plugin._storage.IncrementPlayCount(beatmapId);
                Incremented = true;
            }
        }
    }
}
