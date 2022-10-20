using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatmapPlayCount.Utils
{
    internal class BundledResources
    {
        public static Sprite PlayCountSprite { get; private set; }

        internal static void Load()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BeatmapPlayCount.Resources.Bundle.bundle");
            var assetBundle = AssetBundle.LoadFromStream(stream);

            PlayCountSprite = assetBundle.LoadAsset<Sprite>("PlayCount.png");

            assetBundle.Unload(/* unloadAllLoadedObjects: */ false);
        }
    }
}
