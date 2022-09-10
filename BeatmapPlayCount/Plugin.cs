using System.Reflection;
using IPA;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using SiraUtil.Zenject;
using BeatmapPlayCount.Installers;
using BeatmapPlayCount.Managers;
using Zenject;

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
		/// <summary>
		/// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
		/// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
		/// Only use [Init] with one Constructor.
		/// </summary>
		public void Init(Zenjector zenjector, IPALogger logger) {
			Instance = this;
			Log = logger;

			zenjector.Install<PlayCountGameInstaller>(Location.GameCore);
        }

		#region BSIPA Config
		//Uncomment to use BSIPA's config
		/*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
		#endregion

		[OnStart]
		public void OnApplicationStart() {
			storage = new Storage();

            harmony = new Harmony("Netux.BeatSaber.BeatmapPlayCount");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

		[OnExit]
		public void OnApplicationQuit() {
			harmony.UnpatchSelf();
		}
	}
}
