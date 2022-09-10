using HarmonyLib;
using UnityEngine;
using TMPro;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace BeatmapPlayCount.HarmonyPatches.UI
{
    [HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.RefreshContent))]
    public class PlayCountText
    {
        internal static TextMeshProUGUI playCountTextGameObject { get; private set; }

        static void Postfix(StandardLevelDetailView __instance)
        {
            if (!playCountTextGameObject)
            {
                // Get reference text GameObject
                LevelParamsPanel levelParamsPanel = IPA.Utilities.ReflectionUtil.GetField<LevelParamsPanel, StandardLevelDetailView>(__instance, "_levelParamsPanel");
                TextMeshProUGUI notesPerSecondText = IPA.Utilities.ReflectionUtil.GetField<TextMeshProUGUI, LevelParamsPanel>(levelParamsPanel, "_notesPerSecondText");

                // Create text GameObject from reference
                playCountTextGameObject = GameObject.Instantiate(notesPerSecondText, __instance.transform);
                playCountTextGameObject.name = "PlayCountText";
                playCountTextGameObject.transform.localPosition = new Vector3(20f, -4.5f, 0f);

                var textComp = playCountTextGameObject.GetComponent<TextMeshProUGUI>();
                textComp.fontStyle = FontStyles.Normal;
                textComp.alignment = TextAlignmentOptions.Right;
                textComp.enableWordWrapping = false;
            }

            var beatmap = IPA.Utilities.ReflectionUtil.GetField<IBeatmapLevel, StandardLevelDetailView>(__instance, "_level");
            var count = Plugin.storage.GetPlayCount(beatmap.levelID);

            playCountTextGameObject.text = "\uD83D\uDF82 " + count.ToString();
        }
    }
}