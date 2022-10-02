using BeatmapPlayCount.Configuration;
using Zenject;

namespace BeatmapPlayCount.Managers
{
    internal class TrackPlaytime : IInitializable, ITickable
    {
        private readonly AudioTimeSyncController audioSyncController;
        private readonly string beatmapId;

        public bool Incremented { get; private set; }
        public bool IsGameplayAnExternalModReplay { get; private set; }

        public TrackPlaytime(IDifficultyBeatmap _currentlyPlayingLevel, AudioTimeSyncController _audioSyncController)
        {
            audioSyncController = _audioSyncController;
            beatmapId = _currentlyPlayingLevel.level.levelID;
        }

        public void Initialize()
        {
            Incremented = false;
            IsGameplayAnExternalModReplay = Utils.ExternalReplayMod.CheckIfGameplayIsAReplay();
        }

        public void Tick()
        {
            if (IsGameplayAnExternalModReplay || Incremented)
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
