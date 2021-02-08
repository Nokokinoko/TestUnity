using UnityEngine;

public abstract class AbstractSceneConductor : MonoBehaviour
{
  public abstract Constant.ENUM_SCENE IdScene { get; }

  protected bool m_ImReady = false;
  public bool ImReady { get { return m_ImReady; } }

  protected void Started()
  {
    m_ImReady = true;
    SingletonSceneLoader.Instance.PreloadScene();
  }
}
