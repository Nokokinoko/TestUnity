using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIButton : MonoBehaviour
{
  private readonly string NAME_BUTTON = "Button";
  private readonly string NAME_TEXT = "TextButton";

  private void Start()
  {
    // button
    Transform _ObjButton = transform.Find(NAME_BUTTON);
    if (_ObjButton == null)
    {
      Debug.Log("require " + NAME_BUTTON + " gameobject");
      return;
    }

    MyButton _MyButton = _ObjButton.GetComponent<MyButton>();
    if (_MyButton == null)
    {
      Debug.Log("require MyButton component");
      return;
    }

    // text
    GameObject _ObjText = transform.Find(NAME_TEXT).gameObject;
    if (_ObjText == null)
    {
      Debug.Log("require " + NAME_TEXT + " gameobject");
      return;
    }

    _MyButton.RxOnClick.Subscribe(_ => {
      _ObjText.SetActive(!_ObjText.activeSelf);
    }).AddTo(this);
  }
}
