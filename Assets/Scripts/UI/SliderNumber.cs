using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SliderNumber : MonoBehaviour
{
  private readonly string NAME_SLIDER = "Slider";
  private readonly string NAME_TEXT = "TextSlider";

  private readonly int MIN_VALUE = 0;
  private readonly int MAX_VALUE = 100;

  private void Start()
  {
    // slider
    Transform _ObjSlider = transform.Find(NAME_SLIDER);
    if (_ObjSlider == null)
    {
      Debug.Log("require " + NAME_SLIDER + " gameobject");
      return;
    }

    MySlider _MySlider = _ObjSlider.GetComponent<MySlider>();
    if (_MySlider == null)
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

    Text _Text = _ObjText.GetComponent<Text>();
    if (_Text == null)
    {
      Debug.Log("require Text component");
      return;
    }

    _MySlider.RxOnValueChanged
      .Where(value => (MIN_VALUE <= value && value <= MAX_VALUE))
      .Subscribe(value => {
        _Text.text = value.ToString();
      })
    ;
  }
}
