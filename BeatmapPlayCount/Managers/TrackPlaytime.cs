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
        public bool IsGameplayInPracticeMode { get; private set; }
        public bool DoesBeatmapHaveBannedCharacteristic { get; private set; }

        public TrackPlaytime(
            IDifficultyBeatmap _currentlyPlayingLevel,
            AudioTimeSyncController _audioSyncController,
            GameplayCoreSceneSetupData _gameplayCoreSceneSetupData,
            ILevelEndActions _levelEndActionImpl
           )
        {
            audioSyncController = _audioSyncController;
            beatmapId = _currentlyPlayingLevel.level.levelID;
            beatmapCharacteristic = _currentlyPlayingLevel.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;

            IsGameplayInPracticeMode = _gameplayCoreSceneSetupData.practiceSettings != null;

            _levelEndActionImpl.levelFinishedEvent += handleLevelFinishedEvent;
        }

        public void Initialize()
        {
            Incremented = false;
            IsGameplayAnExternalModReplay = Utils.ExternalReplayMod.CheckIfGameplayIsAReplay();
            DoesBeatmapHaveBannedCharacteristic = PluginConfig.Instance.BannedBeatmapCharacteristics
                .Any(bannedCharacteristic => bannedCharacteristic == beatmapCharacteristic);

#if DEBUG
            Plugin.Log.Info($"TrackPlaytime IsGameplayAnExternalModReplay = {IsGameplayAnExternalModReplay}; DoesBeatmapHaveBannedCharacteristic = {DoesBeatmapHaveBannedCharacteristic}; IsPracticeMode {IsGameplayInPracticeMode}");
#endif
        }

        public void IncrementPlayCount()
        {
            if (Incremented)
            {
                return;
            }

            if (IsGameplayInPracticeMode && !PluginConfig.Instance.IncrementCountInPracticeMode)
            {
                return;
            }

            Plugin._storage.IncrementPlayCount(beatmapId);
            Incremented = true;
        }

        private void handleLevelFinishedEvent()
        {
            if (!Incremented)
            {
                IncrementPlayCount();
            }
        }

        public void Tick()
        {
            if (IsGameplayAnExternalModReplay ||
                DoesBeatmapHaveBannedCharacteristic ||
                IsGameplayInPracticeMode /* handled by handleLevelFinishedEvent */ ||
                Incremented)
            {
                return;
            }

            var progress = audioSyncController.songTime / audioSyncController.songEndTime;
            if (progress >= PluginConfig.Instance.MinimumSongProgressToIncrementingPlayCount)
            {
                IncrementPlayCount();
            }
        }
    }
}
