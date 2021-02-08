using UnityEngine;

public class BridgeSceneConductor : AbstractSceneConductor
{
  public override Constant.ENUM_SCENE IdScene { get { return Constant.ENUM_SCENE.SCENE_BRIDGE; } }

  private readonly string NAME_FADE = "Fade";

  private Fader m_Fader = null;
  public Fader Fader
  {
    get
    {
      if (m_Fader == null)
      {
        GameObject _Canvas = GameObject.Find(Constant.UI_CANVAS);
        if (_Canvas == null)
        {
          Debug.Log("require " + Constant.UI_CANVAS + " gameobject");
          return null;
        }

        Transform _Fade = _Canvas.transform.Find(NAME_FADE);
        if (_Fade == null)
        {
          Debug.Log("require " + NAME_FADE + " gameobject");
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

  private void Start()
  {
    Fader.ToImageEnabled(false);
    Started();
  }
}
