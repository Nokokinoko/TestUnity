using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
  private Transform m_TransformCamera;

  private readonly float INITIAL_THETA = 0.0f;
  private readonly float INITIAL_PHI = 30.0f;
  private readonly float MIN_PHI = 10.0f;
  private readonly float MAX_PHI = 60.0f;

  public Transform m_TransformTarget;
  public float m_Radius = 5.0f;
  public float m_Theta;
  public float m_Phi;
  public float m_SpeedInput = 5.0f;

  private void Start()
  {
    m_TransformCamera = Camera.main.transform;
    SetPositionReset();
  }

  public void SetPositionReset()
  {
    m_Theta = INITIAL_THETA;
    m_Phi = INITIAL_PHI;
    SetPositionByTrackPlayer(); // is default position
  }

  // Playerを中心に追跡するようにカメラ位置計算
  public void SetPositionByTrackPlayer()
  {
    Vector3 _Position = new Vector3(
      m_Radius * Mathf.Cos(m_Phi * Mathf.Deg2Rad) * Mathf.Sin(m_Theta * Mathf.Deg2Rad),
      m_Radius * Mathf.Sin(m_Phi * Mathf.Deg2Rad),
      m_Radius * Mathf.Cos(m_Phi * Mathf.Deg2Rad) * Mathf.Cos(m_Theta * Mathf.Deg2Rad)
    );
    m_TransformCamera.position = _Position + m_TransformTarget.position;

    // look at target
    m_TransformCamera.LookAt(m_TransformTarget);
  }

  // Playerを中心とした球体座標上のカメラ移動
  public void SetPositionByMoveCamera(Vector2 p_InputXY)
  {
    // calc theta
    float _Theta = m_Theta + (p_InputXY.x * m_SpeedInput);
    if (_Theta < 0.0f) { _Theta = 360.0f + _Theta; }
    if (360.0f < _Theta) { _Theta = _Theta - 360.0f; }
    m_Theta = _Theta;

    // calc phi
    float _Phi = m_Phi + (-p_InputXY.y * m_SpeedInput);
    if (_Phi < MIN_PHI) { _Phi = MIN_PHI; }
    if (MAX_PHI < _Phi) { _Phi = MAX_PHI; }
    m_Phi = _Phi;

    SetPositionByTrackPlayer();
  }
}
