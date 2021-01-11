using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
  [ReadOnly]
  public Vector2 m_Move = Vector2.zero;
  [ReadOnly]
  public bool m_Run = false;
  [ReadOnly]
  public bool m_Jump = false;
  [ReadOnly]
  public Vector2 m_Camera = Vector2.zero;
  [ReadOnly]
  public bool m_ResetCamera = false;

  private void Update()
  {
    // move
    Vector2 _Move = Vector2.zero;
    if (Input.GetKey(KeyCode.W))
    {
      _Move += Vector2.up;
    }
    if (Input.GetKey(KeyCode.A))
    {
      _Move += Vector2.left;
    }
    if (Input.GetKey(KeyCode.S))
    {
      _Move += Vector2.down;
    }
    if (Input.GetKey(KeyCode.D))
    {
      _Move += Vector2.right;
    }
    m_Move = _Move.normalized;

    // run
    m_Run = false;
    if (Input.GetKey(KeyCode.LeftShift))
    {
      m_Run = true;
    }

    // jump
    m_Jump = false;
    if (Input.GetKey(KeyCode.Space))
    {
      m_Jump = true;
    }

    // camera
    Vector2 _Camera = Vector2.zero;
    _Camera.x = Input.GetAxis("Mouse X");
    _Camera.y = Input.GetAxis("Mouse Y");
    m_Camera = _Camera;

    m_ResetCamera = false;
    if (Input.GetMouseButton(1))
    {
      m_ResetCamera = true;
    }
  }

  public bool HasInputMove()
  {
    return m_Move != Vector2.zero;
  }

  public bool HasInputCamera()
  {
    return m_Camera != Vector2.zero;
  }
}
