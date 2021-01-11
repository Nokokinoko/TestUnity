﻿using System.Collections;
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
  private readonly float RATIO_RUN = 1.5f;
  private readonly float SPEED_JUMP = 8.0f;

  private float m_LastY;

  private void Start()
  {
    m_Transform = transform;
    m_ModelAnimationCtrl = GetComponent<MyModelAnimationController>();
    m_CharCtrl = GetComponent<CharacterController>();
    m_LastY = m_CharCtrl.center.y;
  }

  private void Update()
  {
    Vector3 _Direction = new Vector3(0.0f, Physics.gravity.y * Time.deltaTime);
    m_CharCtrl.Move(_Direction * Time.deltaTime);

    float _Y = m_CharCtrl.center.y;
    if(_Y < m_LastY && ! m_ModelAnimationCtrl.IsFall())
    {
      // fall
      m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL);
    }
    m_LastY = _Y;

    if(m_CharCtrl.isGrounded)
    {
      if(m_ModelAnimationCtrl.IsFall())
      {
        // land
        m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND);
      }
    }
  }

  public void Idle()
  {
    switch(m_ModelAnimationCtrl.m_StateAnime)
    {
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP:
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL:
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND:
        // can not idle
        return;
      default:
        m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE);
        break;
    }
  }

  public void Rotate(Vector2 p_Move, Vector3 p_PositionCamera)
  {
    if(m_ModelAnimationCtrl.m_StateAnime == Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND)
    {
      // can not rotate
      return;
    }

    float _AngleInput = Mathf.Atan2(p_Move.x, p_Move.y) * Mathf.Rad2Deg;

    Vector3 _VectorCameraToModel = m_Transform.position - p_PositionCamera;
    float _AngleCamera = Mathf.Atan2(_VectorCameraToModel.x, _VectorCameraToModel.z) * Mathf.Rad2Deg;

    m_Transform.rotation = Quaternion.Slerp(
      m_Transform.rotation,
      Quaternion.Euler(0.0f, _AngleInput + _AngleCamera, 0.0f),
      Time.deltaTime * SPEED_ROTATE
    );
  }

  public void Move(Vector2 p_Move, bool p_Run, bool p_Jump)
  {
    Vector3 _Direction = Vector3.zero;
    bool _DoAnime = false;
    Constant.ENUM_STATE_ANIME _StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_WALK;

    // move
    if(p_Move != Vector2.zero)
    {
      if(m_ModelAnimationCtrl.m_StateAnime == Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND)
      {
        // can not move
      }
      else
      {
        _DoAnime = true;
        _Direction.x = p_Move.x * SPEED_MOVE;
        _Direction.z = p_Move.y * SPEED_MOVE;
        if (p_Run)
        {
          _Direction *= RATIO_RUN;
          _StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_RUN;
        }
        _Direction = m_Transform.TransformDirection(_Direction);
      }
    }

    // jump
    if(p_Jump)
    {
      switch(m_ModelAnimationCtrl.m_StateAnime)
      {
        case Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP:
        case Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL:
        case Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND:
          // can not jump
          break;
        default:
          _DoAnime = true;
          _Direction.y = SPEED_JUMP;
          _StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP;
          break;
      }
    }

    if(_DoAnime)
    {
      m_CharCtrl.Move(_Direction * Time.deltaTime);
      m_ModelAnimationCtrl.Animation(_StateAnime);
    }
  }
}
