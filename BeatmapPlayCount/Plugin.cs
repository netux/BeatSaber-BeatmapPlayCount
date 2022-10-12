using System.Reflection;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using SiraUtil.Zenject;
using BeatmapPlayCount.Managers;
using BeatmapPlayCount.Views;
using Zenject;

namespace BeatmapPlayCount
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
	{
		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }
		internal static Storage _storage { get; private set; }

		private static Harmony _harmony { get; set; }

		[Init]
		public void Init(Zenjector zenjector, IPALogger logger) {
			Instance = this;
			Log = logger;

			zenjector.Install(Location.Menu, Container =>
			{
                Container.BindInterfacesAndSelfTo<SettingViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
            });
			zenjector.Install(Location.StandardPlayer, Container =>
			{
                Container.BindInterfacesTo<TrackPlaytime>().AsSingle();
            });
        }

        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
        }

		[OnStart]
		public void OnApplicationStart()
		{
			_storage = new Storage();

            _harmony = new Harmony("site.netux.dev.BeatSaber.BeatmapPlayCount");
			_harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

		[OnExit]
		public void OnApplicationQuit()
		{
			_harmony.UnpatchSelf();
		}
	}
}
