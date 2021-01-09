using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyModelAnimationController : MonoBehaviour
{
  private readonly short ID_BASE_LAYER = 0;
  private readonly string ANIME_FALL = "Fall";

  private readonly string PRM_TO_IDLE = "toIdle";
  private readonly string PRM_TO_WALK = "toWalk";
  private readonly string PRM_TO_RUN = "toRun";
  private readonly string PRM_TO_BACK = "toBack";
  private readonly string PRM_TO_JUMP = "toJump";
  private readonly string PRM_TO_FALL = "toFall";
  private readonly string PRM_TO_LAND = "toLand";

  private Animator m_Animator;
  private Constant.ENUM_STATE_ANIME m_StateAnime;

  private void Start()
  {
    m_Animator = GetComponent<Animator>();
    m_StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE;
  }

  private void Update()
  {
    if(m_StateAnime == Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP)
    {
      if(m_Animator.GetCurrentAnimatorStateInfo(ID_BASE_LAYER).IsName(ANIME_FALL))
      {
        // Jump -> Fall
        m_StateAnime = Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL;
      }
    }
  }

  public void Animation(Constant.ENUM_STATE_ANIME p_StateAnime)
  {
    if(m_StateAnime == p_StateAnime)
    {
      return;
    }

    string _Parameter = "";
    switch(p_StateAnime)
    {
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_IDLE:
        _Parameter = PRM_TO_IDLE;
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_WALK:
        _Parameter = PRM_TO_WALK;
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_RUN:
        _Parameter = PRM_TO_RUN;
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_BACK:
        _Parameter = PRM_TO_BACK;
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_JUMP:
        _Parameter = PRM_TO_JUMP;
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_FALL:
        _Parameter = PRM_TO_FALL;
        break;
      case Constant.ENUM_STATE_ANIME.STATE_ANIME_LAND:
        _Parameter = PRM_TO_LAND;
        break;
    }
    m_Animator.SetTrigger(_Parameter);
    m_StateAnime = p_StateAnime;
  }
}
