using UnityEngine;
using UniRx;

public class BulletObjectPoolProvider : MonoBehaviour
{
  [SerializeField]
  private Bullet m_PrefabBullet;

  private BulletObjectPool m_BulletObjectPool = null;
  public BulletObjectPool BulletObjectPool
  {
    get
    {
      if (m_BulletObjectPool == null)
      {
        m_BulletObjectPool = new BulletObjectPool(m_PrefabBullet);
        m_BulletObjectPool.PreloadAsync(preloadCount: 20, threshold: 20).Subscribe();
      }
      return m_BulletObjectPool;
    }
  }

  private void OnDestroy()
  {
    m_BulletObjectPool.Dispose();
  }
}
