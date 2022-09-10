using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace BeatmapPlayCount.Managers
{
    internal class TrackPlaytime : IInitializable, ITickable
    {
        private readonly AudioTimeSyncController audioSyncController;
        private readonly string beatmapId;

        public bool Incremented { get; private set; }

        private float percentageToIncrement = 0.7f; // TODO(netux): make configurable

        public TrackPlaytime(IDifficultyBeatmap _currentlyPlayingLevel, AudioTimeSyncController _audioSyncController)
        {
            audioSyncController = _audioSyncController;
            beatmapId = _currentlyPlayingLevel.level.levelID;
        }

        public void Initialize()
        {
            Incremented = false;
        }

        public void Tick()
        {
            if (Incremented)
            {
                return;
            }

            var progress = audioSyncController.songTime / audioSyncController.songEndTime;
            if (progress >= this.percentageToIncrement)
            {
                Plugin.storage.IncrementPlayCount(beatmapId);
                Incremented = true;
            }
        }
    }
}
