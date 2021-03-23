using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(MyModelAnimationController))]
[RequireComponent(typeof(CharacterController))]
abstract public class AbstractModelInputController : MonoBehaviour
{
    protected readonly float SPEED_ROTATE = 10.0f;

    protected Transform m_Transform;
    protected MyModelAnimationController m_ModelAnimationCtrl;
    protected CharacterController m_CharCtrl;

    protected void Awake()
    {
        m_Transform = transform;
        m_ModelAnimationCtrl = GetComponent<MyModelAnimationController>();
        m_CharCtrl = GetComponent<CharacterController>();
    }

    abstract public void Rotate(Vector2 p_Move, Vector3 p_ForwardCamera);
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
