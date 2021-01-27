using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIButton : MonoBehaviour
{
  private readonly string NAME_BUTTON = "Button";
  private readonly string NAME_TEXT = "TextButton";

  private MyButton m_MyButton;
  private GameObject m_ObjText;
  private bool m_IsActiveText;

  private void Start()
  {
    // button
    Transform _ObjButton = transform.Find(NAME_BUTTON);
    if (_ObjButton == null)
    {
      Debug.Log("require " + NAME_BUTTON + " gameobject");
      return;
    }

    m_MyButton = _ObjButton.GetComponent<MyButton>();
    if (m_MyButton == null)
    {
      Debug.Log("require MyButton component");
      return;
    }

    // text
    m_ObjText = transform.Find(NAME_TEXT).gameObject;
    if (m_ObjText == null)
    {
      Debug.Log("require " + NAME_TEXT + " gameobject");
      return;
    }

    m_IsActiveText = m_ObjText.activeSelf;

    m_MyButton.RxOnClick.Subscribe(_ => {
      m_ObjText.SetActive(!m_IsActiveText);
      m_IsActiveText = m_ObjText.activeSelf;
    }).AddTo(gameObject);
  }
}
