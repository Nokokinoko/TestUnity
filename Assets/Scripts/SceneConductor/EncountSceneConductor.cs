using System;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(InputReceiver))]
[RequireComponent(typeof(CameraManager))]
public class EncountSceneConductor : AbstractSceneConductor
{
    public override Constant.ENUM_SCENE IdScene { get { return Constant.ENUM_SCENE.SCENE_ENCOUNT; } }

    private readonly string NAME_MODEL = "MyModel";
    private readonly string NAME_BUTTON_ENCOUNT = "ButtonEncount";

    public enum ENUM_DIMENSION
    {
        DIMENSION_2,
        DIMENSION_3,
    };
    public ENUM_DIMENSION m_Dimension;

    private bool m_DisableInput = false;
    private bool m_IsEncount = false;
    private InputReceiver m_InputReceiver;
    private CameraManager m_CameraMgr;

    private AbstractModelInputController m_ModelInputCtrl;
    private MyButton m_ButtonEncount;

    private void Awake()
    {
        m_InputReceiver = GetComponent<InputReceiver>();
        m_CameraMgr = GetComponent<CameraManager>();
        m_CameraMgr.TransformCamera = TransformCamera;
    }

    private void Start()
    {
        // Model
        GameObject _Model = GameObject.Find(NAME_MODEL);
        if (_Model == null)
        {
            Debug.Log("require " + NAME_MODEL + " gameobject");
            return;
        }

        if (Is2D())
        {
            // 2D
            m_ModelInputCtrl = _Model.GetComponent<Model2DInputController>();
            _Model.GetComponent<Model3DInputController>().enabled = false;
            ((Model2DInputController)m_ModelInputCtrl).OffCharaCtrl();
        }
        else
        {
            // 3D
            m_ModelInputCtrl = _Model.GetComponent<Model3DInputController>();
            _Model.GetComponent<Model2DInputController>().enabled = false;
        }
        if (m_ModelInputCtrl == null)
        {
            Debug.Log("require AbstractModelInputController component");
            return;
        }

        // Button
        Transform _ObjButton = Canvas.transform.Find(NAME_BUTTON_ENCOUNT);
        if (_ObjButton == null)
        {
            Debug.Log("require " + NAME_BUTTON_ENCOUNT + " gameobject");
            return;
        }

        m_ButtonEncount = _ObjButton.GetComponent<MyButton>();
        if (m_ButtonEncount == null)
        {
            Debug.Log("require MyButton component (" + NAME_BUTTON_ENCOUNT + ")");
            return;
        }

        m_ButtonEncount.Inactive();

        Started();
    }

    private void Update()
    {
        if (Is2D() && ((Model2DInputController)m_ModelInputCtrl).IsMoving())
        {
            m_CameraMgr.SetPositionByTrackPlayer();
        }
        if (m_DisableInput || m_IsEncount)
        {
            return;
        }

        // for model
        if (m_InputReceiver.HasInputMove())
        {
            if (Is2D())
            {
                m_DisableInput = true;
                ((Model2DInputController)m_ModelInputCtrl).RxMoved.Subscribe(_ => { m_DisableInput = false; }).AddTo(this);
            }
            // カメラ向き固定だが他シーンでカメラ移動を考慮しているため流用
            m_ModelInputCtrl.Rotate(m_InputReceiver.m_Move, TransformCamera.forward);
            m_ModelInputCtrl.Move(m_InputReceiver.m_Move, TransformCamera);
            if (!Is2D())
            {
                m_CameraMgr.SetPositionByTrackPlayer();
            }
        }
        else
        {
            m_ModelInputCtrl.Idle();
        }
    }

    private bool Is2D()
    {
        return (m_Dimension == ENUM_DIMENSION.DIMENSION_2);
    }

    public void Encount()
    {
        m_IsEncount = true;
        m_ModelInputCtrl.Stop();
        Observable.Interval(TimeSpan.FromMilliseconds(200))
            .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(1)))
            .Subscribe(
                _ =>
                {
                    // 簡易的に点滅
                    if (m_ButtonEncount.isActiveAndEnabled)
                    {
                        m_ButtonEncount.Inactive();
                    }
                    else
                    {
                        m_ButtonEncount.Active();
                    }
                },
                () =>
                {
                    m_ButtonEncount.Active();
                    m_ButtonEncount.RxOnClick
                        .Subscribe(_ => {
                            m_IsEncount = false;
                            m_ButtonEncount.Inactive();
                            m_ModelInputCtrl.Resume();
                        })
                        .AddTo(this)
                    ;
                }
            )
            .AddTo(this)
        ;
    }
}
