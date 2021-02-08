using UnityEngine;
using UniRx;

public class CanvasConductor : MonoBehaviour
{
  private readonly string NAME_PANEL_FADE = "PanelFade";
  private readonly string NAME_BUTTON_MENU = "ButtonMenu";
  public readonly string NAME_PANEL_MENU = "PanelMenu";

  public GameObject Canvas { private get; set; }

  private Fader m_Fader = null;
  public Fader Fader
  {
    get
    {
      if (m_Fader == null)
      {
        Transform _Fade = Canvas.transform.Find(NAME_PANEL_FADE);
        if (_Fade == null)
        {
          Debug.Log("require " + NAME_PANEL_FADE + " gameobject");
          return null;
        }

        m_Fader = _Fade.GetComponent<Fader>();
        if (m_Fader == null)
        {
          Debug.Log("require Fader component");
          return null;
        }
      }
      return m_Fader;
    }
  }

  private MyButton m_ButtonMenu;
  private PanelMenu m_PanelMenu;

  private void Start()
  {
    // PanelFade
    Fader.FadeOut();

    // ButtonMenu
    Transform _ObjButton = Canvas.transform.Find(NAME_BUTTON_MENU);
    if (_ObjButton == null)
    {
      Debug.Log("require " + NAME_BUTTON_MENU + " gameobject");
      return;
    }

    m_ButtonMenu = _ObjButton.GetComponent<MyButton>();
    if (m_ButtonMenu == null)
    {
      Debug.Log("require MyButton component");
      return;
    }

    // PanelMenu
    Transform _Menu = Canvas.transform.Find(NAME_PANEL_MENU);
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

    m_ButtonMenu.RxOnClick.Subscribe(_ => {
      m_ButtonMenu.Inactive();
      m_PanelMenu.Active();
    }).AddTo(this);

    m_PanelMenu.RxClose.Subscribe(_ => {
      InactiveMenu();
    }).AddTo(this);

    InactiveMenu();
  }

  public void InactiveMenu()
  {
    m_PanelMenu.Inactive();
    m_ButtonMenu.Active();
  }

  public bool IsActiveMenu()
  {
    return m_PanelMenu.isActiveAndEnabled;
  }
}
