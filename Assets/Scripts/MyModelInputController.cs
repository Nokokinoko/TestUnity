using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyModelAnimationController))]
public class MyModelInputController : MonoBehaviour
{
  private MyModelAnimationController m_ModelAnimationCtrl;

  private void Start()
  {
    m_ModelAnimationCtrl = GetComponent<MyModelAnimationController>();
  }

  public void Move(Vector2 p_Model, bool p_Run)
  {

  }

  public void Jump()
  {

  }
}
