using Zenject;

public class ArrowKeyInputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<IInputProvider>()
            .To<ArrowKeyInputProvider>()
            .AsSingle();
    }
}