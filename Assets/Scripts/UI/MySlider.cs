using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Slider))]
public class MySlider : MonoBehaviour
{
  private Slider m_Slider;

  private Subject<float> m_RxOnValueChanged = new Subject<float>();
  public IObservable<float> RxOnValueChanged { get { return m_RxOnValueChanged.AsObservable(); } }

  private void Start()
  {
    m_Slider = GetComponent<Slider>();
    m_Slider.onValueChanged.AddListener(value => {
      m_RxOnValueChanged.OnNext(value);
    });
  }
}
