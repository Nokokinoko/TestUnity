using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyModelAnimationController : MonoBehaviour
{
  private readonly short ID_BASE_LAYER = 0;
  private readonly string ANIME_IDLE = "Idle";

  private readonly string PRM_TO_WALK = "toWalk";
  private readonly string PRM_TO_RUN = "toRun";
  private readonly string PRM_TRIGGER_JUMP = "triggerJump";
  private readonly string PRM_TRIGGER_FALL = "triggerFall";
  private readonly string PRM_TRIGGER_LAND = "triggerLand";

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
    if(m_StateAnime == p_StateAnime)
    {
      return;
    }

    switch(p_StateAnime)
    {
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE:
        m_Animator.SetBool(PRM_TO_WALK, false);
        m_Animator.SetBool(PRM_TO_RUN, false);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_WALK:
        m_Animator.SetBool(PRM_TO_WALK, true);
        m_Animator.SetBool(PRM_TO_RUN, false);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_RUN:
        m_Animator.SetBool(PRM_TO_WALK, false);
        m_Animator.SetBool(PRM_TO_RUN, true);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP:
        m_Animator.SetBool(PRM_TO_WALK, false);
        m_Animator.SetBool(PRM_TO_RUN, false);
        m_Animator.SetTrigger(PRM_TRIGGER_JUMP);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL:
        m_Animator.SetBool(PRM_TO_WALK, false);
        m_Animator.SetBool(PRM_TO_RUN, false);
        m_Animator.SetTrigger(PRM_TRIGGER_FALL);
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND:
        m_Animator.SetBool(PRM_TO_WALK, false);
        m_Animator.SetBool(PRM_TO_RUN, false);
        m_Animator.SetTrigger(PRM_TRIGGER_LAND);
        break;
    }
    m_StateAnime = p_StateAnime;
  }

  public bool IsFall()
  {
    return m_StateAnime == Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL;
  }
}
