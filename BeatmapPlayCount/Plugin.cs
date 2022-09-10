using System.Reflection;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using SiraUtil.Zenject;
using BeatmapPlayCount.Installers;

namespace BeatmapPlayCount
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
	{
		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }
		internal static Storage storage { get; private set; }

		private static Harmony harmony { get; set; }

		[Init]
		public void Init(Zenjector zenjector, IPALogger logger) {
			Instance = this;
			Log = logger;

			zenjector.Install<PlayCountGameInstaller>(Location.GameCore);
        }

        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
        }

		[OnStart]
		public void OnApplicationStart()
		{
			storage = new Storage();

            harmony = new Harmony("Netux.BeatSaber.BeatmapPlayCount");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

		[OnExit]
		public void OnApplicationQuit()
		{
			harmony.UnpatchSelf();
		}
	}
}
