using System.Threading;
using Cysharp.Threading.Tasks;

public interface ISkippable<T> where T : class
{
    public UniTask OnStart(T args, CancellationToken token);
    public UniTask OnSkip(T args, CancellationToken token);
}
