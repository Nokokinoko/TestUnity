using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

public class InputReceiver : MonoBehaviour
{
  private readonly string INPUT_AXIS_MOUSE_X = "Mouse X";
  private readonly string INPUT_AXIS_MOUSE_Y = "Mouse Y";
  private readonly int INPUT_BUTTON_MOUSE_LEFT = 0;
  private readonly int INPUT_BUTTON_MOUSE_RIGHT = 1;

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

  private readonly PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
  private readonly List<RaycastResult> m_ListRaycast = new List<RaycastResult>();

  private readonly Subject<Unit> m_RxButtonMouseLeft = new Subject<Unit>();
  public IObservable<Unit> RxButtonMouseLeft { get { return m_RxButtonMouseLeft.AsObservable(); } }

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
    _Camera.x = Input.GetAxis(INPUT_AXIS_MOUSE_X);
    _Camera.y = Input.GetAxis(INPUT_AXIS_MOUSE_Y);
    m_Camera = _Camera;

    m_ResetCamera = false;
    if (Input.GetMouseButtonDown(INPUT_BUTTON_MOUSE_RIGHT))
    {
      m_ResetCamera = true;
    }

    // mouse
    if (Input.GetMouseButtonDown(INPUT_BUTTON_MOUSE_LEFT))
    {
      m_ListRaycast.Clear();
      m_PointerEventData.position = Input.mousePosition;
      EventSystem.current.RaycastAll(m_PointerEventData, m_ListRaycast);
      m_RxButtonMouseLeft.OnNext(Unit.Default);
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

  public bool IsInGameObjectByName(string pName)
  {
    foreach (RaycastResult _Result in m_ListRaycast)
    {
      if (_Result.gameObject.name.Equals(pName))
      {
        return true;
      }
    }
    return false;
  }
}
