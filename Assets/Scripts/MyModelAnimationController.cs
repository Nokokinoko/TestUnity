using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyModelAnimationController : MonoBehaviour
{
  private readonly short ID_BASE_LAYER = 0;
  private readonly string ANIME_IDLE = "Idle";
  private readonly string ANIME_RUN = "Run";
  private readonly string ANIME_JUMP = "Jump";
  private readonly string ANIME_FALL = "Fall";

  private readonly string PRM_TO_WALK = "toWalk";
  private readonly string PRM_TO_RUN = "toRun";
  private readonly string PRM_TO_JUMP = "toJump";
  private readonly string PRM_TO_FALL = "toFall";
  private readonly string PRM_TO_LAND = "toLand";

  private Animator m_Animator;
  [ReadOnly]
  public Constant.ENUM_STATE_ANIME m_StateAnime;

  private void Start()
  {
    m_Animator = GetComponent<Animator>();
    m_StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE;
  }

  private void Update()
  {
    if (m_StateAnime == Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND)
    {
      if (m_Animator.GetCurrentAnimatorStateInfo(ID_BASE_LAYER).IsName(ANIME_IDLE))
      {
        // land to idle
        m_StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE;
      }
    }
  }

  public void Animation(Constant.ENUM_STATE_ANIME p_StateAnime)
  {
    if (m_StateAnime == p_StateAnime)
    {
      return;
    }

    foreach (AnimatorControllerParameter _Prm in m_Animator.parameters)
    {
      if (_Prm.type == AnimatorControllerParameterType.Bool)
      {
        m_Animator.SetBool(_Prm.name, false);
      }
    }
    switch (p_StateAnime)
    {
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_WALK:
        m_Animator.SetBool(PRM_TO_WALK, true);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_RUN:
        m_Animator.SetBool(PRM_TO_RUN, true);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP:
        m_Animator.SetBool(PRM_TO_JUMP, true);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL:
        m_Animator.SetBool(PRM_TO_FALL, true);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND:
        m_Animator.SetBool(PRM_TO_LAND, true);
        break;
      default:
        // idle
        break;
    }
    m_StateAnime = p_StateAnime;
  }

  public bool IsRunning()
  {
    return m_Animator.GetCurrentAnimatorStateInfo(ID_BASE_LAYER).IsName(ANIME_RUN);
  }

  public bool IsJumping()
  {
    return m_Animator.GetCurrentAnimatorStateInfo(ID_BASE_LAYER).IsName(ANIME_JUMP);
  }

  public bool IsFalling()
  {
    return m_Animator.GetCurrentAnimatorStateInfo(ID_BASE_LAYER).IsName(ANIME_FALL);
  }

  public bool IsAir()
  {
    switch (m_StateAnime)
    {
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP:
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL:
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND:
        return true;
      default:
        break;
    }
    return false;
  }
}
