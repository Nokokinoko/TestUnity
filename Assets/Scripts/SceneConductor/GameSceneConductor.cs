using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(InputReceiver))]
[RequireComponent(typeof(CameraManager))]
[RequireComponent(typeof(CanvasConductor))]
public class GameSceneConductor : AbstractSceneConductor
{
  public override Constant.ENUM_SCENE IdScene { get { return Constant.ENUM_SCENE.SCENE_GAME; } }

  private readonly string NAME_MODEL = "MyModel";
  private readonly float DROP_Y = -2.0f;
  private readonly int TIME_DROP = 500; // 0.5sec

  private bool m_DisableInput = false;
  private InputReceiver m_InputReceiver;

  private MyModelInputController m_ModelInputCtrl;

  private Transform m_TransformCamera;
  private CameraManager m_CameraMgr;

  private CanvasConductor m_CanvasConductor;

  private void Awake()
  {
    m_InputReceiver = GetComponent<InputReceiver>();
    m_CameraMgr = GetComponent<CameraManager>();
    m_CanvasConductor = GetComponent<CanvasConductor>();
    m_CanvasConductor.Canvas = Canvas;
  }

  private void Start()
  {
    m_TransformCamera = Camera.main.transform;

    GameObject _Model = GameObject.Find(NAME_MODEL);
    if (_Model == null)
    {
      Debug.Log("require " + NAME_MODEL + " gameobject");
      return;
    }

    m_ModelInputCtrl = _Model.GetComponent<MyModelInputController>();
    if (m_ModelInputCtrl == null)
    {
      Debug.Log("require MyModelInputController component");
      return;
    }

    m_InputReceiver.RxButtonMouseLeft
      .Where(_ => m_CanvasConductor.IsActiveMenu())
      .Where(_ => !m_InputReceiver.IsInGameObjectByName(m_CanvasConductor.NAME_PANEL_MENU))
      .Subscribe(_ => m_CanvasConductor.InactiveMenu())
      .AddTo(this)
    ;

    m_CanvasConductor.Fader.RxCompletedFadeIn.Subscribe(_ => {
      m_ModelInputCtrl.ResetPosition();
      m_CameraMgr.SetPositionReset();
      m_DisableInput = false;

      m_CanvasConductor.Fader.FadeOut();
    }).AddTo(this);

    this.UpdateAsObservable()
      .Where(_ => !m_DisableInput)
      .Where(_ => _Model.transform.position.y < DROP_Y)
      .Subscribe(_ => Drop())
    ;

    Started();
  }

  private void Update()
  {
    if (m_DisableInput)
    {
      return;
    }
    if (m_CanvasConductor.IsActiveMenu())
    {
      return;
    }

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

  private void Drop()
  {
    m_DisableInput = true;

    m_CameraMgr.SetPositionOverhead();

    Observable
      .Timer(TimeSpan.FromMilliseconds(TIME_DROP))
      .Subscribe(_ => {
        m_CanvasConductor.Fader.FadeIn(); // 完了時にRxCompletedFadeInを発火
      })
    ;
  }
}
