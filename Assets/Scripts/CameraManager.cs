using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class CameraManager : MonoBehaviour
{
  private readonly string NAME_FADE = "PanelFade";
  private readonly Color COLOR_FADE = new Color(0.0f, 0.0f, 0.0f);
  private readonly float TIME_FADE = 1.0f;

  private readonly float INITIAL_THETA = 0.0f;
  private readonly float INITIAL_PHI = 30.0f;
  private readonly float MIN_PHI = 10.0f;
  private readonly float MAX_PHI = 60.0f;
  private readonly float MIN_POSITION_Y = 0.5f;
  private readonly float OVERHEAD_POSITION_Y = 3.0f;

  private Transform m_TransformCamera;
  private Image m_Fade;
  private float m_TimeFade = 0.0f;
  private bool m_IsFadeIn = false;
  private bool m_IsFadeOut = false;

  private Subject<bool> m_RxCompletedFadeIn = new Subject<bool>();
  public IObservable<bool> RxCompletedFadeIn { get { return m_RxCompletedFadeIn.AsObservable(); } }

  public Transform m_TransformTarget;
  public float m_Radius = 5.0f;
  public float m_Theta;
  public float m_Phi;
  public float m_SpeedInput = 5.0f;

  private void Start()
  {
    m_TransformCamera = Camera.main.transform;
    SetPositionReset();

    GameObject _Canvas = GameObject.Find(Constant.NAME_CANVAS);
    if (_Canvas == null)
    {
      Debug.Log("require "+Constant.NAME_CANVAS+" gameobject");
      return;
    }

    Transform _Fade = _Canvas.transform.Find(NAME_FADE);
    if (_Fade == null)
    {
      Debug.Log("require "+NAME_FADE+" gameobject");
      return;
    }

    m_Fade = _Fade.GetComponent<Image>();
    if (m_Fade == null)
    {
      Debug.Log("require Image component");
      return;
    }

    this.UpdateAsObservable()
      .Where(_ => m_IsFadeIn || m_IsFadeOut)
      .Subscribe(_ => UpdateFade())
    ;

    FadeOut();
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
    _Position += m_TransformTarget.position;
    if (_Position.y < MIN_POSITION_Y)
    {
      _Position.y = MIN_POSITION_Y;
    }
    m_TransformCamera.position = _Position;

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

  // Player頭上にカメラ移動
  public void SetPositionOverhead()
  {
    Vector3 _Position = m_TransformTarget.position;
    _Position.y = OVERHEAD_POSITION_Y;
    m_TransformCamera.position = _Position;

    m_TransformCamera.rotation = Quaternion.Euler(90.0f, -180.0f, 0.0f);
  }

  public void FadeIn()
  {
    // 0 -> 1
    Color _Color = COLOR_FADE;
    _Color.a = 0.0f;
    m_Fade.color = _Color;
    m_Fade.enabled = true;

    m_TimeFade = 0.0f;
    m_IsFadeIn = true;
  }

  public void FadeOut()
  {
    // 1 -> 0
    Color _Color = COLOR_FADE;
    _Color.a = 1.0f;
    m_Fade.color = _Color;
    m_Fade.enabled = true;

    m_TimeFade = 0.0f;
    m_IsFadeOut = true;
  }

  private void UpdateFade()
  {
    bool _IsFinish = false;
    m_TimeFade += Time.deltaTime;
    if (TIME_FADE < m_TimeFade)
    {
      _IsFinish = true;
      m_TimeFade = TIME_FADE;
    }

    float _Per = m_TimeFade / TIME_FADE;
    Color _Color = COLOR_FADE;
    _Color.a = m_IsFadeIn ? _Per : 1.0f - _Per;
    m_Fade.color = _Color;

    if (!_IsFinish)
    {
      // updating yet
      return;
    }

    bool _IsFadeIn = m_IsFadeIn;
    m_IsFadeIn = false;
    m_IsFadeOut = false;

    if (_IsFadeIn)
    {
      // SceneConductorで登録された処理でFadeOutを呼び出すため先にフラグの初期化を済ませておく
      m_RxCompletedFadeIn.OnNext(true);
    }
    else
    {
      m_Fade.enabled = false;
    }
  }
}
