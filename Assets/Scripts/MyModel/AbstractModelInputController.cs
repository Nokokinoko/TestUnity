using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(MyModelAnimationController))]
[RequireComponent(typeof(CharacterController))]
abstract public class AbstractModelInputController : MonoBehaviour
{
    private readonly float SPEED_ROTATE = 10.0f;

    protected Transform m_Transform;
    protected MyModelAnimationController m_ModelAnimationCtrl;
    protected CharacterController m_CharCtrl;

    protected void Awake()
    {
        m_Transform = transform;
        m_ModelAnimationCtrl = GetComponent<MyModelAnimationController>();
        m_CharCtrl = GetComponent<CharacterController>();
    }

    abstract protected bool CanRotate(Vector2 p_Move, Vector3 p_ForwardCamera);

    public void Rotate(Vector2 p_Move, Vector3 p_ForwardCamera)
    {
        if (!CanRotate(p_Move, p_ForwardCamera))
        {
            return;
        }

        float _AngleInput = Mathf.Atan2(p_Move.x, p_Move.y) * Mathf.Rad2Deg;
        float _AngleCamera = Mathf.Atan2(p_ForwardCamera.x, p_ForwardCamera.z) * Mathf.Rad2Deg;

        m_Transform.rotation = Quaternion.Slerp(
            m_Transform.rotation,
            Quaternion.Euler(0.0f, _AngleInput + _AngleCamera, 0.0f),
            Time.deltaTime * SPEED_ROTATE
        );
    }

    abstract public void Move(Vector2 p_Move, Transform p_TransformCamera);

    abstract protected bool CanIdle();

    public void Idle()
    {
        if (CanIdle())
        {
            m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE);
        }
    }
}
