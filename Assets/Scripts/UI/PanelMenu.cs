using System;
using UnityEngine;
using UniRx;

public class PanelMenu : MonoBehaviour
{
  private readonly string NAME_BUTTON_CLOSE = "ButtonClose";
  private readonly string NAME_BUTTON_GO_TO = "ButtonGoTo";

  private MyButton m_ButtonClose = null;
  private MyButton ButtonClose
  {
    get
    {
      if (m_ButtonClose == null)
      {
        try
        {
          Transform _ObjButton = transform.Find(NAME_BUTTON_CLOSE);
          if (_ObjButton == null)
          {
            throw new Exception("require " + NAME_BUTTON_CLOSE + " gameobject");
          }

          m_ButtonClose = _ObjButton.GetComponent<MyButton>();
          if (m_ButtonClose == null)
          {
            throw new Exception("require MyButton component (" + NAME_BUTTON_CLOSE + ")");
          }
        }
        catch (Exception e)
        {
          Debug.Log(e.Message);
        }
      }
      return m_ButtonClose;
    }
  }

  public IObservable<Unit> RxClose { get { return ButtonClose.RxOnClick; } }

  private void Start()
  {
    // ButtonGoTo
    Transform _ObjButton = transform.Find(NAME_BUTTON_GO_TO);
    if (_ObjButton == null)
    {
      Debug.Log("require " + NAME_BUTTON_GO_TO + " gameobject");
      return;
    }

    MyButton _Button = _ObjButton.GetComponent<MyButton>();
    if (_Button == null)
    {
      Debug.Log("require MyButton component (" + NAME_BUTTON_GO_TO + ")");
      return;
    }

    _Button.RxOnClick
      .First()
      .Subscribe(_ => SingletonSceneLoader.Instance.GoToObjectPool())
      .AddTo(this)
    ;
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
