using BeatmapPlayCount.Managers;
using Zenject;

// TODO(netux): remove
namespace BeatmapPlayCount.Installers
{
    public class PlayCountGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TrackPlaytime>().AsSingle();
        }
    }
}
