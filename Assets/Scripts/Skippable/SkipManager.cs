using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class SkipManager<T> where T : class
{
    public class Parameter
    {
        public ISkippable<T> skippable = null;
        public T args = null;

        public IObservable<Unit> trigger = null;
        public CancellationToken token;

        public Parameter(ISkippable<T> skippable, T args, IObservable<Unit> trigger, CancellationToken token)
        {
            this.skippable = skippable;
            this.args = args;
            this.trigger = trigger;
            this.token = token;
        }
    }

    protected Parameter _parameter = null;
    protected CancellationTokenSource _cts;
    protected IDisposable _disposable = null;

    public SkipManager(Parameter parameter)
    {
        _parameter = parameter;
        _cts = new CancellationTokenSource();
        _disposable = _parameter.trigger.Subscribe(_ => _cts.Cancel());
    }

    public async UniTask Process()
    {
        // cancelがリクエストされたらOperationCanceledExceptionを投げる
        // UniTaskが管理しているUniTaskを中断させる例外
        // if (token.IsCancellationRequested()) { return; }と同義
        _parameter.token.ThrowIfCancellationRequested();

        // どちらかがキャンセルされたらキャンセルが飛ぶ
        CancellationTokenSource linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, _parameter.token);

        bool isSkipped = await _parameter.skippable.OnStart(_parameter.args, linkedCts.Token)
            .SuppressCancellationThrow();

        if (isSkipped)
        {
            // スキップ
            await _parameter.skippable.OnSkip(_parameter.args, _parameter.token);
            Debug.Log("is Skipped.");
        }
        else
        {
            // 完了
            Debug.Log("is Complete.");
        }
        
        _disposable?.Dispose();
    }
}
