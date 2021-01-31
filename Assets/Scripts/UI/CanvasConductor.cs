using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class CanvasConductor : MonoBehaviour
{
  private readonly string NAME_CANVAS = "Canvas";
  private readonly string NAME_PANEL_FADE = "PanelFade";
  private readonly string NAME_BUTTON_MENU = "ButtonMenu";
  private readonly string NAME_PANEL_MENU = "PanelMenu";

  private readonly Color COLOR_FADE = new Color(0.0f, 0.0f, 0.0f);
  private readonly float TIME_FADE = 1.0f;

  private Image m_Fade;
  private float m_TimeFade = 0.0f;
  private bool m_IsFadeIn = false;
  private bool m_IsFadeOut = false;

  private Subject<Unit> m_RxCompletedFadeIn = new Subject<Unit>();
  public IObservable<Unit> RxCompletedFadeIn { get { return m_RxCompletedFadeIn.AsObservable(); } }

  private PanelMenu m_PanelMenu;

  private void Start()
  {
    GameObject _Canvas = GameObject.Find(NAME_CANVAS);
    if (_Canvas == null)
    {
      Debug.Log("require " + NAME_CANVAS + " gameobject");
      return;
    }

    // PanelFade
    Transform _Fade = _Canvas.transform.Find(NAME_PANEL_FADE);
    if (_Fade == null)
    {
      Debug.Log("require " + NAME_PANEL_FADE + " gameobject");
      return;
    }

    m_Fade = _Fade.GetComponent<Image>();
    if (m_Fade == null)
    {
      Debug.Log("require Image component");
      return;
    }

    this.UpdateAsObservable()
      .Where(_ => m_IsFadeIn || m_IsFadeOut)
      .Subscribe(_ => UpdateFade())
    ;

    FadeOut();

    // ButtonMenu
    Transform _ObjButton = _Canvas.transform.Find(NAME_BUTTON_MENU);
    if (_ObjButton == null)
    {
      Debug.Log("require " + NAME_BUTTON_MENU + " gameobject");
      return;
    }

    MyButton _ButtonMenu = _ObjButton.GetComponent<MyButton>();
    if (_ButtonMenu == null)
    {
      Debug.Log("require MyButton component");
      return;
    }

    // PanelMenu
    Transform _Menu = _Canvas.transform.Find(NAME_PANEL_MENU);
    if (_Menu == null)
    {
      Debug.Log("require " + NAME_PANEL_MENU + " gameobject");
      return;
    }

    m_PanelMenu = _Menu.GetComponent<PanelMenu>();
    if (m_PanelMenu == null)
    {
      Debug.Log("require PanelMenu component");
      return;
    }

    _ButtonMenu.RxOnClick.Subscribe(_ => {
      _ButtonMenu.Inactive();
      m_PanelMenu.Active();
    }).AddTo(this);

    m_PanelMenu.RxClose.Subscribe(_ => {
      m_PanelMenu.Inactive();
      _ButtonMenu.Active();
    }).AddTo(this);

    m_PanelMenu.Inactive();
    _ButtonMenu.Active();
  }

  public void FadeIn()
  {
    // 0 -> 1
    Color _Color = COLOR_FADE;
    _Color.a = 0.0f;
    m_Fade.color = _Color;
    m_Fade.enabled = true;

    m_TimeFade = 0.0f;
    m_IsFadeIn = true;
  }

  public void FadeOut()
  {
    // 1 -> 0
    Color _Color = COLOR_FADE;
    _Color.a = 1.0f;
    m_Fade.color = _Color;
    m_Fade.enabled = true;

    m_TimeFade = 0.0f;
    m_IsFadeOut = true;
  }

  private void UpdateFade()
  {
    bool _IsFinish = false;
    m_TimeFade += Time.deltaTime;
    if (TIME_FADE < m_TimeFade)
    {
      _IsFinish = true;
      m_TimeFade = TIME_FADE;
    }

    float _Per = m_TimeFade / TIME_FADE;
    Color _Color = COLOR_FADE;
    _Color.a = m_IsFadeIn ? _Per : 1.0f - _Per;
    m_Fade.color = _Color;

    if (!_IsFinish)
    {
      // updating yet
      return;
    }

    bool _IsFadeIn = m_IsFadeIn;
    m_IsFadeIn = false;
    m_IsFadeOut = false;

    if (_IsFadeIn)
    {
      // SceneConductorで登録された処理でFadeOutを呼び出すため先にフラグの初期化を済ませておく
      m_RxCompletedFadeIn.OnNext(Unit.Default);
    }
    else
    {
      m_Fade.enabled = false;
    }
  }

  public bool IsActiveMenu()
  {
    return m_PanelMenu.isActiveAndEnabled;
  }
}
