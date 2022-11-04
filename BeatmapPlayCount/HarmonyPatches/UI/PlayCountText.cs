using HarmonyLib;
using UnityEngine;
using BeatmapPlayCount.Utils;
using TMPro;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace BeatmapPlayCount.HarmonyPatches.UI
{
    [HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.RefreshContent))]
    public class PlayCountText
    {
        internal static GameObject playCountContainerGameObject { get; private set; }
        internal static TMPro.TextMeshProUGUI playCountText { get; private set; }

        static void Postfix(StandardLevelDetailView __instance)
        {
            if (!playCountContainerGameObject)
            {
                // Get reference GameObject
                var levelParamsPanel = IPA.Utilities.ReflectionUtil.GetField<LevelParamsPanel, StandardLevelDetailView>(__instance, "_levelParamsPanel");
                var notesPerSecondText = IPA.Utilities.ReflectionUtil.GetField<TextMeshProUGUI, LevelParamsPanel>(levelParamsPanel, "_notesPerSecondText");
                playCountContainerGameObject = notesPerSecondText.transform.parent.gameObject;

                // Create play count text from reference
                playCountContainerGameObject = GameObject.Instantiate(
                    playCountContainerGameObject,
                    playCountContainerGameObject.transform.parent.parent /* LevelDetail game object */
                );
                playCountContainerGameObject.name = "Play Count";

                var playCountContainerHLG = playCountContainerGameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                playCountContainerHLG.spacing = 0.5f;
                playCountContainerHLG.childForceExpandWidth = false;
                playCountContainerHLG.childForceExpandHeight = false;
                playCountContainerHLG.childAlignment = TextAnchor.MiddleRight;

                Object.Destroy(playCountContainerGameObject.GetComponent<HMUI.HoverHint>());     // remove as this is causing exception spam
                Object.Destroy(playCountContainerGameObject.GetComponent<LocalizedHoverHint>()); // when hovering over the label

                var playCountIconGameObject = playCountContainerGameObject.transform.Find("Icon").gameObject;
                playCountIconGameObject.GetComponent<HMUI.ImageView>().sprite = BundledResources.PlayCountSprite;

                var playCountTextGameObject = playCountContainerGameObject.transform.Find("ValueText").gameObject;

                playCountText = playCountTextGameObject.GetComponent<TextMeshProUGUI>();
                playCountText.fontStyle = FontStyles.Normal;
                playCountText.alignment = TextAlignmentOptions.Right;
                playCountText.enableWordWrapping = false;
            }

            var beatmap = IPA.Utilities.ReflectionUtil.GetField<IBeatmapLevel, StandardLevelDetailView>(__instance, "_level");
            var count = Plugin._storage.GetPlayCount(beatmap.levelID);

            playCountText.text = count.ToString();

            playCountContainerGameObject.transform.localPosition = new Vector3(14f, -3f, 0f);
            if (SongCore.UI.RequirementsUI.instance.ButtonInteractable)
            {
                playCountContainerGameObject.transform.localPosition += Vector3.left * 10;
            }
        }
    }
}