using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Button))]
public class MyButton : MonoBehaviour
{
  private Button m_Button = null;
  private Button acs_Button
  {
    get
    {
      if (m_Button == null)
      {
        m_Button = GetComponent<Button>();
      }
      return m_Button;
    }
  }

  public IObservable<Unit> RxOnClick { get { return acs_Button.OnClickAsObservable(); } }

  public void Active()
  {
    gameObject.SetActive(true);
  }

  public void Inactive()
  {
    gameObject.SetActive(false);
  }
}
