using System;
using UnityEngine;
using UniRx;

public class Enemy : MonoBehaviour
{
  [SerializeField]
  private BulletObjectPoolProvider m_BulletObjectPoolProvider;
  private BulletObjectPool m_BulletObjectPool;

  private void Start()
  {
    m_BulletObjectPool = m_BulletObjectPoolProvider.BulletObjectPool;

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
      // インスタンスを取得
      Bullet _Bullet = m_BulletObjectPool.Rent();

      // 3way
      Vector3 _Direction = Quaternion.AngleAxis(i * 30.0f, transform.up) * transform.forward;
      Vector3 _Position = transform.position + _Direction * 1.0f;
      _Bullet.Initialize(_Position, _Direction * 3.0f);
      _Bullet.OnFinishedSubject
        .Take(1)
        .Subscribe(_ => {
          m_BulletObjectPool.Return(_Bullet);
        })
      ;
    }
  }
}
