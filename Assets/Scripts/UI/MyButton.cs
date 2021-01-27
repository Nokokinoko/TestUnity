using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Button))]
public class MyButton : MonoBehaviour
{
  private Button m_Button;

  private Subject<Unit> m_RxOnClick = new Subject<Unit>();
  public IObservable<Unit> RxOnClick { get { return m_RxOnClick.AsObservable(); } }

  private void Start()
  {
    m_Button = GetComponent<Button>();
    m_Button.onClick.AddListener(() => {
      m_RxOnClick.OnNext(Unit.Default);
    });
  }

  public void Active()
  {
    m_Button.gameObject.SetActive(true);
  }

  public void Inactive()
  {
    m_Button.gameObject.SetActive(false);
  }
}
