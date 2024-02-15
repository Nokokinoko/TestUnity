using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class TextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [Inject] private IInputProvider _input;
    
    private void Awake()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => UpdateText())
            .AddTo(this);
    }

    private void UpdateText()
    {
        if (_input.Up())
        {
            text.text = "Up";
        }
        else if (_input.Right())
        {
            text.text = "Right";
        }
        else if (_input.Down())
        {
            text.text = "Down";
        }
        else if (_input.Left())
        {
            text.text = "Left";
        }
        else
        {
            text.text = "";
        }
    }
}
