using BeatmapPlayCount.Views;
using Zenject;

// TODO(netux): remove
namespace BeatmapPlayCount.Installers
{
    public class PlayCountMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log.Debug($"PlayCountMenuInstaller InstallBindings"); // TODO(netux): remove debug log
            Container.Bind<SettingViewController>().FromNewComponentAsViewController().AsSingle();
        }
    }
}
