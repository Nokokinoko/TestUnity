using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Slider))]
public class MySlider : MonoBehaviour
{
  private Slider m_Slider = null;
  private Slider Slider
  {
    get
    {
      if (m_Slider == null)
      {
        m_Slider = GetComponent<Slider>();
      }
      return m_Slider;
    }
  }

  public IObservable<float> RxOnValueChanged { get { return Slider.OnValueChangedAsObservable(); } }
}
