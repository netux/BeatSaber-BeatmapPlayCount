using BeatmapPlayCount.Managers;
using Zenject;

namespace BeatmapPlayCount.Installers
{
    public class PlayCountGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TrackPlaytime>().AsSingle();
        }
    }
}
