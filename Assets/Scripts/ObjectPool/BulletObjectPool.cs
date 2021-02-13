using UnityEngine;
using UniRx.Toolkit;

public class BulletObjectPool : ObjectPool<Bullet>
{
  private readonly Bullet m_PrefabBullet;
  private readonly Transform m_TransformParent;

  // constructor
  public BulletObjectPool(Bullet pPrefab)
  {
    m_PrefabBullet = pPrefab;

    m_TransformParent = new GameObject().transform;
    m_TransformParent.name = "Bullets";
    m_TransformParent.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
  }

  protected override Bullet CreateInstance()
  {
    Bullet _Bullet = GameObject.Instantiate(m_PrefabBullet);
    _Bullet.transform.SetParent(m_TransformParent);
    return _Bullet;
  }
}
