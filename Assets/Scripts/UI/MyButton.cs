using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Button))]
public class MyButton : MonoBehaviour
{
  private Button m_Button;

  public IObservable<Unit> RxOnClick { get { return m_Button.OnClickAsObservable(); } }

  private void Awake()
  {
    m_Button = GetComponent<Button>();
  }

  public void Active()
  {
    gameObject.SetActive(true);
  }

  public void Inactive()
  {
    gameObject.SetActive(false);
  }
}
