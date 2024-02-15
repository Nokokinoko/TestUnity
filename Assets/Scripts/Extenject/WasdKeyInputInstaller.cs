using Zenject;

public class WasdKeyInputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<IInputProvider>()
            .To<WasdKeyInputProvider>()
            .AsSingle();
    }
}