using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
  private Rigidbody m_Rigidbody;

  private readonly Subject<Unit> m_OnFinishedSubject = new Subject<Unit>();
  public IObservable<Unit> OnFinishedSubject { get { return m_OnFinishedSubject.AsObservable(); } }

  private void Start()
  {
    m_Rigidbody = GetComponent<Rigidbody>();

    // 衝突
    this.OnTriggerEnterAsObservable()
      .Subscribe(_ => OnHit())
    ;
  }

  public void Initialize(Vector3 pPosition, Vector3 pVelocity)
  {
    transform.position = pPosition;

    Observable
      .NextFrame(FrameCountType.FixedUpdate)
      .TakeUntilDisable(this)
      .Subscribe(_ => m_Rigidbody.AddForce(pVelocity, ForceMode.VelocityChange))
      .AddTo(this)
    ;

    Observable
      .Timer(TimeSpan.FromSeconds(3))
      .TakeUntilDisable(this)
      .TakeUntilDestroy(this)
      .Subscribe(_ => Finish())
    ;
  }

  private void OnHit()
  {
    Finish();
  }

  private void Finish()
  {
    m_Rigidbody.velocity = Vector3.zero;
    m_OnFinishedSubject.OnNext(Unit.Default);
  }

  private void OnDestroy()
  {
    m_OnFinishedSubject.Dispose();
  }
}
