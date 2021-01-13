using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputReceiver))]
[RequireComponent(typeof(CameraManager))]
public class SceneConductor : MonoBehaviour
{
  private InputReceiver m_InputReceiver;

  private MyModelInputController m_ModelInputCtrl;

  private Transform m_TransformCamera;
  private CameraManager m_CameraMgr;

  private void Start()
  {
    m_InputReceiver = GetComponent<InputReceiver>();
    m_CameraMgr = GetComponent<CameraManager>();

    m_TransformCamera = Camera.main.transform;

    GameObject _Model = GameObject.Find("MyModel");
    if (_Model == null)
    {
      Debug.Log("require MyModel gameobject");
      return;
    }

    m_ModelInputCtrl = _Model.GetComponent<MyModelInputController>();
    if (m_ModelInputCtrl == null)
    {
      Debug.Log("require MyModelInputController component");
      return;
    }
  }

  private void Update()
  {
    // for model
    bool _UpdatePosition = false;
    if (m_InputReceiver.HasInputMove() || m_InputReceiver.m_Jump)
    {
      if (m_InputReceiver.HasInputMove())
      {
        m_ModelInputCtrl.Rotate(m_InputReceiver.m_Move, m_TransformCamera.forward);
      }
      m_ModelInputCtrl.Move(m_InputReceiver.m_Move, m_InputReceiver.m_Run, m_InputReceiver.m_Jump, m_TransformCamera);
      _UpdatePosition = true;
    }

    if (m_ModelInputCtrl.IsAir())
    {
      // jump, fall, land
      _UpdatePosition = true;
    }

    if (!m_InputReceiver.HasInputMove() && !m_InputReceiver.m_Jump)
    {
      m_ModelInputCtrl.Idle();
    }

    // for camera
    if (m_InputReceiver.m_ResetCamera)
    {
      m_CameraMgr.SetPositionReset();
      _UpdatePosition = false; // SetPositionResetで更新するので不要
    }
    else if (m_InputReceiver.HasInputCamera())
    {
      m_CameraMgr.SetPositionByMoveCamera(m_InputReceiver.m_Camera);
      _UpdatePosition = false; // SetPositionByMoveCameraで更新するので不要
    }

    if (_UpdatePosition)
    {
      m_CameraMgr.SetPositionByTrackPlayer();
    }
  }
}
