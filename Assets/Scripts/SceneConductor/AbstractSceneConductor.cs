using UnityEngine.Assertions;
using UnityEngine;

public abstract class AbstractSceneConductor : MonoBehaviour
{
  public abstract Constant.ENUM_SCENE IdScene { get; }

  private bool m_ImReady = false;
  public bool ImReady { get { return m_ImReady; } }

  private Transform m_TransformCamera = null;
  protected Transform TransformCamera
  {
    get
    {
      if (m_TransformCamera == null)
      {
        foreach (GameObject _Obj in GameObject.FindGameObjectsWithTag(Constant.TAG_MAIN_CAMERA))
        {
          ComponentAttachScene _Attach = _Obj.GetComponent<ComponentAttachScene>();
          if (_Attach.AttachIdScene == IdScene)
          {
            m_TransformCamera = _Obj.transform;
            break;
          }
        }
        Assert.IsNotNull(m_TransformCamera, "MainCamera is not found");
      }
      return m_TransformCamera;
    }
  }

  private GameObject m_Canvas = null;
  protected GameObject Canvas
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
    if (IdScene != Constant.ENUM_SCENE.SCENE_BRIDGE)
    {
      SingletonSceneLoader.Instance.PreloadScene(IdScene);
    }
  }
}
