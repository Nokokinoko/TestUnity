using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SkippableCountDown : MonoBehaviour, ISkippable<SkippableCountDown.Args>
{
    public class Args
    {
        public int count;
        public string defaultCompleteText = "";
        public string skipCompleteText = "";

        public Args(int count, string defaultCompleteText, string skipCompleteText)
        {
            this.count = count;
            this.defaultCompleteText = defaultCompleteText;
            this.skipCompleteText = skipCompleteText;
        }
    }

    [SerializeField] protected TextMeshProUGUI _stateText = null;

    public async UniTask OnStart(Args args, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        for (int i = args.count; 0 <= i; i--)
        {
            _stateText.text = i.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        }

        _stateText.text = args.defaultCompleteText;
    }

    public async UniTask OnSkip(Args args, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        _stateText.text = args.skipCompleteText;
        await UniTask.CompletedTask;
    }
}
