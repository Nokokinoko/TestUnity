using UnityEngine;
using UniRx;

[RequireComponent(typeof(InputReceiver))]
[RequireComponent(typeof(CameraManager))]
public class EncountSceneConductor : AbstractSceneConductor
{
    public override Constant.ENUM_SCENE IdScene { get { return Constant.ENUM_SCENE.SCENE_ENCOUNT; } }

    private readonly string NAME_MODEL = "MyModel";

    public enum ENUM_DIMENSION
    {
        DIMENSION_2,
        DIMENSION_3,
    };
    public ENUM_DIMENSION m_Dimension;

    private bool m_DisableInput = false;
    private InputReceiver m_InputReceiver;

    private AbstractModelInputController m_ModelInputCtrl;

    private CameraManager m_CameraMgr;

    private void Awake()
    {
        m_InputReceiver = GetComponent<InputReceiver>();
        m_CameraMgr = GetComponent<CameraManager>();
        m_CameraMgr.TransformCamera = TransformCamera;
    }

    private void Start()
    {
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

        Started();
    }

    private void Update()
    {
        if (Is2D() && ((Model2DInputController)m_ModelInputCtrl).IsMoving())
        {
            m_CameraMgr.SetPositionByTrackPlayer();
        }
        if (m_DisableInput)
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
}
