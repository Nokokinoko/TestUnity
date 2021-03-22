using UnityEngine;

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

        m_ModelInputCtrl = (m_Dimension == ENUM_DIMENSION.DIMENSION_2)
            ? _Model.GetComponent<Model3DInputController>()
            : _Model.GetComponent<Model2DInputController>()
        ;
        if (m_ModelInputCtrl == null)
        {
            Debug.Log("require AbstractModelInputController component");
            return;
        }

        Started();
    }

    private void Update()
    {
        if (m_DisableInput)
        {
            return;
        }

        // for model
        if (m_InputReceiver.HasInputMove())
        {
            m_ModelInputCtrl.Rotate(m_InputReceiver.m_Move, TransformCamera.forward);
            m_ModelInputCtrl.Move(m_InputReceiver.m_Move, TransformCamera);
            m_CameraMgr.SetPositionByTrackPlayer();
        }
        else
        {
            m_ModelInputCtrl.Idle();
        }
    }
}
