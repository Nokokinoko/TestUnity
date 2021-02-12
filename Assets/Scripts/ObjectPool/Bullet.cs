using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
  private Rigidbody _Rigidbody;

  private void Start()
  {
    _Rigidbody = GetComponent<Rigidbody>();

    // 衝突したら削除
    this.OnTriggerEnterAsObservable()
      .Subscribe(_ => Destroy(gameObject))
    ;
    // 3秒経過したら削除
    Destroy(gameObject, 3.0f);
  }

  public void AddVelocity(Vector3 pVelocity)
  {
    Observable.NextFrame(FrameCountType.FixedUpdate)
      .Subscribe(_ => _Rigidbody.AddForce(pVelocity, ForceMode.VelocityChange))
      .AddTo(this)
    ;
  }
}
