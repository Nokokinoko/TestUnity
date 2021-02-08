using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
  public Color m_ColorDefault;
  public float m_TimeFade;

  private Image m_ImageFade;
  private Image ImageFade
  {
    get
    {
      if(m_ImageFade == null)
      {
        m_ImageFade = GetComponent<Image>();
      }
      return m_ImageFade;
    }
  }

  private float m_Time = 0.0f;
  private bool m_IsFadeIn = false;
  private bool m_IsFadeOut = false;

  private readonly Subject<Unit> m_RxCompletedFadeIn = new Subject<Unit>();
  public IObservable<Unit> RxCompletedFadeIn { get { return m_RxCompletedFadeIn.AsObservable(); } }

  private void Start()
  {
    this.UpdateAsObservable()
      .Where(_ => m_IsFadeIn || m_IsFadeOut)
      .Subscribe(_ => UpdateFade())
    ;
  }

  public void ToImageEnabled(bool pEnabled)
  {
    ImageFade.enabled = pEnabled;
  }

  public void FadeIn()
  {
    // 0 -> 1
    Color _Color = m_ColorDefault;
    _Color.a = 0.0f;
    ImageFade.color = _Color;
    ToImageEnabled(true);

    m_Time = 0.0f;
    m_IsFadeIn = true;
  }

  public void FadeOut()
  {
    // 1 -> 0
    Color _Color = m_ColorDefault;
    _Color.a = 1.0f;
    ImageFade.color = _Color;
    ToImageEnabled(true);

    m_Time = 0.0f;
    m_IsFadeOut = true;
  }

  private void UpdateFade()
  {
    bool _IsFinish = false;
    m_Time += Time.deltaTime;
    if (m_TimeFade < m_Time)
    {
      _IsFinish = true;
      m_Time = m_TimeFade;
    }

    float _Per = m_Time / m_TimeFade;
    Color _Color = m_ColorDefault;
    _Color.a = m_IsFadeIn ? _Per : 1.0f - _Per;
    ImageFade.color = _Color;

    if (!_IsFinish)
    {
      // updating yet
      return;
    }

    bool _IsFadeIn = m_IsFadeIn;
    m_IsFadeIn = false;
    m_IsFadeOut = false;

    if (_IsFadeIn)
    {
      m_RxCompletedFadeIn.OnNext(Unit.Default);
    }
    else
    {
      ToImageEnabled(false);
    }
  }
}
