using System;
using UnityEngine;
using UniRx;

public class Enemy : MonoBehaviour
{
  [SerializeField]
  private Bullet m_PrefabBullet;

  private void Start()
  {
    // 1秒毎に弾を発射
    Observable
      .Interval(TimeSpan.FromSeconds(1))
      .Subscribe(_ => ShootBullets())
      .AddTo(this)
    ;
  }

  private void ShootBullets()
  {
    for (int i = -1; i < 2; i++)
    {
      Bullet _Bullet = Instantiate<Bullet>(m_PrefabBullet);

      // 3way
      Vector3 _Direction = Quaternion.AngleAxis(i * 30.0f, transform.up) * transform.forward;
      _Bullet.transform.position += _Direction * 1.0f;
      _Bullet.AddVelocity(_Direction * 3.0f);
    }
  }
}
