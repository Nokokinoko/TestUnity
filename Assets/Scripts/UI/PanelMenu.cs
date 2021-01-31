using System;
using UnityEngine;
using UniRx;

public class PanelMenu : MonoBehaviour
{
  private readonly string NAME_BUTTON_CLOSE = "ButtonClose";

  private MyButton m_ButtonClose = null;
  private MyButton acs_ButtonClose
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
            throw new Exception("require MyButton component");
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

  public IObservable<Unit> RxClose { get { return acs_ButtonClose.RxOnClick; } }

  public void Active()
  {
    gameObject.SetActive(true);
  }

  public void Inactive()
  {
    gameObject.SetActive(false);
  }
}
