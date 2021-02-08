using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Slider))]
public class MySlider : MonoBehaviour
{
  private Slider m_Slider;

  public IObservable<float> RxOnValueChanged { get { return m_Slider.OnValueChangedAsObservable(); } }

  private void Awake()
  {
    m_Slider = GetComponent<Slider>();
  }
}
