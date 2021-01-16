using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(MyModelAnimationController))]
[RequireComponent(typeof(CharacterController))]
public class MyModelInputController : MonoBehaviour
{
  private Transform m_Transform;
  private MyModelAnimationController m_ModelAnimationCtrl;
  private CharacterController m_CharCtrl;

  private readonly float SPEED_ROTATE = 10.0f;
  private readonly float SPEED_MOVE = 6.0f;
  private readonly float RATIO_MOVE_AIR = 0.5f;
  private readonly float RATIO_RUN = 1.5f;
  private readonly float SPEED_JUMP = 8.0f;

  private readonly float DIFF_FALL_Y = 0.01f;
  private readonly float DIRECTION_SIDE = 45.0f;

  private float m_Gravity;
  private float m_LastY;
  private bool m_HitSide = false;

  private void Start()
  {
    m_Transform = transform;
    m_ModelAnimationCtrl = GetComponent<MyModelAnimationController>();
    m_CharCtrl = GetComponent<CharacterController>();
    m_Gravity = 0.0f;
    m_LastY = m_Transform.position.y;
  }

  private void Update()
  {
    if (m_Gravity < 0.0f)
    {
      m_Gravity = 0.0f;
    }
    m_Gravity += Physics.gravity.y * Time.deltaTime;
    m_CharCtrl.Move(new Vector3(0.0f, m_Gravity) * Time.deltaTime);

    float _Y = m_Transform.position.y;
    if (DIFF_FALL_Y < (m_LastY - _Y) && !m_ModelAnimationCtrl.IsFall())
    {
      // fall
      m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL);
    }
    m_LastY = _Y;

    if (m_ModelAnimationCtrl.IsFall() && m_CharCtrl.isGrounded)
    {
      // land
      m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND);
    }
  }

  public void Idle()
  {
    if (!IsAir())
    {
      m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE);
    }
  }

  public void Rotate(Vector2 p_Move, Vector3 p_ForwardCamera)
  {
    if (m_ModelAnimationCtrl.m_StateAnime == Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND)
    {
      // can not rotate
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

  public void Move(Vector2 p_Move, bool p_Run, bool p_Jump, Transform p_TransformCamera)
  {
    bool _DoAnime = false;
    Constant.ENUM_STATE_ANIME _StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_WALK;

    // move
    if (p_Move != Vector2.zero)
    {
      if (m_ModelAnimationCtrl.m_StateAnime == Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND || m_HitSide)
      {
        // can not move
      }
      else
      {
        _DoAnime = true;
        float _X = p_Move.x * SPEED_MOVE;
        float _Y = p_Move.y * SPEED_MOVE;

        if (IsAir())
        {
          // jump, fall, land
          _DoAnime = false;
          _X *= RATIO_MOVE_AIR;
          _Y *= RATIO_MOVE_AIR;
        }
        else if (p_Run)
        {
          if (m_ModelAnimationCtrl.IsRunning())
          {
            // transition completed
            _X *= RATIO_RUN;
            _Y *= RATIO_RUN;
          }
          _StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_RUN;
        }

        Vector3 _ForwardCamera = Vector3.Scale(p_TransformCamera.forward, new Vector3(1.0f, 0.0f, 1.0f)).normalized;
        Vector3 _Direction = _ForwardCamera * _Y + p_TransformCamera.right * _X;
        m_CharCtrl.Move(_Direction * Time.deltaTime);
      }
    }

    // jump
    if (p_Jump)
    {
      if (!IsAir())
      {
        _DoAnime = true;
        m_Gravity = SPEED_JUMP;
        _StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP;
      }
    }

    if (_DoAnime)
    {
      m_ModelAnimationCtrl.Animation(_StateAnime);
    }
  }

  public bool IsAir()
  {
    return m_ModelAnimationCtrl.IsAir();
  }

  private void OnControllerColliderHit(ControllerColliderHit p_Hit)
  {
    Vector3 _Up = m_Transform.TransformDirection(Vector3.up);
    Vector3 _Direction = p_Hit.point - m_Transform.position;
    float _Angle = Vector3.Angle(_Up, _Direction);
    m_HitSide = (_Angle < DIRECTION_SIDE);
  }
}
