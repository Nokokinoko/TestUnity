using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UISlider : MonoBehaviour
{
  private readonly string NAME_SLIDER = "Slider";
  private readonly string NAME_TEXT = "TextSlider";

  private readonly int MIN_VALUE = 0;
  private readonly int MAX_VALUE = 100;

  private MySlider m_MySlider;
  private Text m_Text;

  private void Start()
  {
    // slider
    Transform _ObjSlider = transform.Find(NAME_SLIDER);
    if (_ObjSlider == null)
    {
      Debug.Log("require " + NAME_SLIDER + " gameobject");
      return;
    }

    m_MySlider = _ObjSlider.GetComponent<MySlider>();
    if (m_MySlider == null)
    {
      Debug.Log("require MySlider component");
      return;
    }

    // text
    Transform _ObjText = transform.Find(NAME_TEXT);
    if (_ObjText == null)
    {
      Debug.Log("require " + NAME_TEXT + " gameobject");
      return;
    }

    m_Text = _ObjSlider.GetComponent<Text>();
    if (m_Text == null)
    {
      Debug.Log("require Text component");
      return;
    }

    m_MySlider.RxOnValueChanged
      .Where(value => MIN_VALUE <= value && value <= MAX_VALUE)
      .Subscribe(value => {
        m_Text.text = value.ToString();
      })
    ;
  }
}
