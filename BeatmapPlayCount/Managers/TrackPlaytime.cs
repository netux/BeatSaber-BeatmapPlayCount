using BeatmapPlayCount.Configuration;
using System;
using System.Linq;
using Zenject;

namespace BeatmapPlayCount.Managers
{
    internal class TrackPlaytime : IInitializable, IDisposable, ITickable
    {
        private readonly AudioTimeSyncController audioSyncController;
        private readonly string beatmapId;
        private readonly string beatmapCharacteristic;
        private readonly ILevelEndActions levelEndActionImpl;

        public bool Incremented { get; private set; }
        public bool IsGameplayAnExternalModReplay { get; private set; }
        public bool IsGameplayInPracticeMode { get; private set; }
        public bool DoesBeatmapHaveBannedCharacteristic { get; private set; }

        public float SongStartTime { get; private set; }

        public bool CanIncrementByPercentageBecauseOfPracticeMode
        {
            get
            {
                return IsGameplayInPracticeMode &&
                    PluginConfig.Instance.IncrementCountInPracticeMode &&
                    !PluginConfig.Instance.OnlyIncrementInPracticeModeWhenThePlayerFinishes;
            }
        }

        public bool CanIncrement
        {
            get
            {
                return !IsGameplayAnExternalModReplay &&
                    !DoesBeatmapHaveBannedCharacteristic;
            }
        }

        public bool CanIncrementByPercentage
        {
            get
            {
                return CanIncrement &&
                    CanIncrementByPercentageBecauseOfPracticeMode;
            }
        }

        public TrackPlaytime(
            AudioTimeSyncController _audioSyncController,
            AudioTimeSyncController.InitData _audioSyncControllerInitData,
            GameplayCoreSceneSetupData _gameplayCoreSceneSetupData,
            ILevelEndActions _levelEndActionImpl
           )
        {
            audioSyncController = _audioSyncController;
            beatmapId = _gameplayCoreSceneSetupData.difficultyBeatmap
                .level.levelID;
            beatmapCharacteristic = _gameplayCoreSceneSetupData.difficultyBeatmap
                .parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
            levelEndActionImpl = _levelEndActionImpl;

            IsGameplayInPracticeMode = _gameplayCoreSceneSetupData.practiceSettings != null;
            SongStartTime = _audioSyncControllerInitData.startSongTime;
        }

        public void Initialize()
        {
            Incremented = false;
            IsGameplayAnExternalModReplay = Utils.ExternalReplayMod.CheckIfGameplayIsAReplay();
            DoesBeatmapHaveBannedCharacteristic = PluginConfig.Instance.BannedBeatmapCharacteristics
                .Any(bannedCharacteristic => bannedCharacteristic == beatmapCharacteristic);

            levelEndActionImpl.levelFinishedEvent += handleLevelFinishedEvent;

#if DEBUG
            Plugin.Log.Info($"TrackPlaytime IsGameplayAnExternalModReplay = {IsGameplayAnExternalModReplay}; DoesBeatmapHaveBannedCharacteristic = {DoesBeatmapHaveBannedCharacteristic}; IsPracticeMode {IsGameplayInPracticeMode}; SongStartTime {SongStartTime}");
#endif
        }

        public void Dispose()
        {
            levelEndActionImpl.levelFinishedEvent -= handleLevelFinishedEvent;
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
            if (!CanIncrementByPercentage || Incremented)
            {
                return;
            }

            var currentTime = Math.Max(0, audioSyncController.songTime - SongStartTime);
            var endTime = audioSyncController.songEndTime - SongStartTime;
            var progress = currentTime / endTime;
#if DEBUG
            Plugin.Log.Debug($"Progress {progress}; current time {audioSyncController.songTime}; relative current time {currentTime}; relative duration {endTime}; duration {audioSyncController.songEndTime}");
#endif
            if (progress >= PluginConfig.Instance.MinimumSongProgressToIncrementPlayCount)
            {
                IncrementPlayCount();
            }
        }
    }
}
