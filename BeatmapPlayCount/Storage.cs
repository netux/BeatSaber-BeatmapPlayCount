using System.IO;
using System.Collections.Generic;
using IPA.Utilities;

namespace BeatmapPlayCount
{
    class Storage
    {
        public readonly string basePath;

        readonly Dictionary<string, int> countsCache;

        public Storage()
        {
            basePath = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "PlayCounts");
            countsCache = new Dictionary<string, int>();
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            else
            {
                foreach (string filePath in Directory.EnumerateFiles(basePath))
                {
                    string timesStr = File.ReadAllText(filePath);
                    int times = int.Parse(timesStr);

                    string beatmapId = Path.GetFileNameWithoutExtension(filePath);
                    countsCache[beatmapId] = times;
                }
            }
        }

        public void IncrementPlayCount(string beatmapId)
        {
            countsCache[beatmapId] = GetPlayCount(beatmapId) + 1;
            File.WriteAllText(Path.Combine(basePath, beatmapId + ".count"), countsCache[beatmapId].ToString());
            Plugin.Log.Debug($"Incremented {beatmapId} play count to {countsCache[beatmapId]}");
        }

        public int GetPlayCount(string beatmapId)
        {
            return countsCache.ContainsKey(beatmapId) ? countsCache[beatmapId] : 0;
        }
    }
}
