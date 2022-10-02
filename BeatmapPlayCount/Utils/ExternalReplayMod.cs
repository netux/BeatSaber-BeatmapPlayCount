using IPA.Loader;
using System.Reflection;

namespace BeatmapPlayCount.Utils
{
    internal static class ExternalReplayMod
    {
        private static MethodInfo scoreSaberPatchHandleHMUnmountedPrefix;
        internal static bool IsGameplayScoreSaberReplay()
        {
            // Thanks kinsi55 from Camera2! https://github.com/kinsi55/CS_BeatSaber_Camera2/blob/687335c0e6689db5411398dc534e246ea2d39c32/Utils/Scoresaber.cs#L9
            PluginMetadata scoreSaberInstance = PluginManager.GetPluginFromId("ScoreSaber");
            if (scoreSaberInstance == null)
            {
                return false;
            }

            if (scoreSaberPatchHandleHMUnmountedPrefix == null)
            {
                scoreSaberPatchHandleHMUnmountedPrefix = scoreSaberInstance
                    .Assembly.GetType("ScoreSaber.Core.ReplaySystem.HarmonyPatches.PatchHandleHMDUnmounted")?
                    .GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
                if (scoreSaberPatchHandleHMUnmountedPrefix == null)
                {
                    Plugin.Log.Warn("ScoreSaber replay check failed: Could not find PatchHandleHMDUnmounted.Prefix method.");
                    return false;
                }
            }


            // Looking at this method on dnSpy, it only returns the opposite of some internal state in ScoreSaber.
            // The state has obfuscated names, so this is a nice shortcut.
            return !((bool) scoreSaberPatchHandleHMUnmountedPrefix.Invoke(null, null));
        }

        internal static bool CheckIfGameplayIsAReplay()
        {
            return IsGameplayScoreSaberReplay();
        }
    }
}
