using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SkippableCountDown _countDown;
    [SerializeField] private Button _skipButton;

    private void Start()
    {
        StartCountDown().Forget();
    }

    private async UniTask StartCountDown()
    {
        SkippableCountDown.Args args = new SkippableCountDown.Args(5, "Default\nComplete", "Skip\nComplete");
        SkipManager<SkippableCountDown.Args>.Parameter parameter = new SkipManager<SkippableCountDown.Args>.Parameter(
            skippable: _countDown,
            args: args,
            trigger: _skipButton.OnClickAsObservable(),
            token: _countDown.GetCancellationTokenOnDestroy()
        );
        SkipManager<SkippableCountDown.Args> skipManager = new SkipManager<SkippableCountDown.Args>(parameter);

        await skipManager.Process();
        
        Debug.Log("Finish");
    }
}
