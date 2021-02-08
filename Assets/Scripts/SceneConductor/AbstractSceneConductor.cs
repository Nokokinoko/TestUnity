using UnityEngine;

public abstract class AbstractSceneConductor : MonoBehaviour
{
  public abstract Constant.ENUM_SCENE IdScene { get; }

  private bool m_ImReady = false;
  public bool ImReady { get { return m_ImReady; } }

  private GameObject m_Canvas = null;
  public GameObject Canvas
  {
    get
    {
      if (m_Canvas == null)
      {
        foreach (GameObject _Obj in GameObject.FindGameObjectsWithTag(Constant.TAG_CANVAS))
        {
          ComponentAttachScene _Attach = _Obj.GetComponent<ComponentAttachScene>();
          if (_Attach.AttachIdScene == IdScene)
          {
            m_Canvas = _Obj;
            break;
          }
        }
      }
      return m_Canvas;
    }
  }

  protected void Started()
  {
    m_ImReady = true;
    SingletonSceneLoader.Instance.PreloadScene(IdScene);
  }
}
