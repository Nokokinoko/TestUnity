using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Model2DInputController : AbstractModelInputController
{
    private readonly float TIME_ROTATE = 0.2f;
    private readonly float TIME_MOVE = 0.3f;
    private readonly float RATIO_MOVE = 1.5f;

    private float m_TimeMove = 0.0f;
    private Vector3 m_PositionMoveFrom = Vector3.zero;
    private Vector3 m_PositionMoveAdd = Vector3.zero;

    private readonly Subject<Unit> m_RxMoved = new Subject<Unit>();
    public IObservable<Unit> RxMoved { get { return m_RxMoved.AsObservable(); } }

    private void Update()
    {
        if (!IsMoving())
        {
            return;
        }

        m_TimeMove += Time.deltaTime;
        float _Ratio = Mathf.Clamp01(m_TimeMove / TIME_MOVE);
        m_Transform.position = m_PositionMoveFrom + m_PositionMoveAdd * _Ratio;
        if (1.0f <= _Ratio)
        {
            m_PositionMoveAdd = Vector3.zero;
            m_RxMoved.OnNext(Unit.Default);
        }
    }

    private Vector3 ConvDirection(Vector2 p_Move)
    {
        Vector3 _Direction = Vector3.zero;
        if(1.0f <= Mathf.Abs(p_Move.x))
        {
            _Direction.x = -p_Move.x;
        }
        if(1.0f <= Mathf.Abs(p_Move.y))
        {
            _Direction.z = -p_Move.y;
        }
        return _Direction;
    }

    override public void Rotate(Vector2 p_Move, Vector3 p_ForwardCamera)
    {
        Vector3 _Direction = ConvDirection(p_Move);
        if (_Direction == Vector3.zero)
        {
            return;
        }

        float _TimeRotate = 0.0f;
        float _AngleInput = Mathf.Atan2(p_Move.x, p_Move.y) * Mathf.Rad2Deg;
        float _AngleCamera = Mathf.Atan2(p_ForwardCamera.x, p_ForwardCamera.z) * Mathf.Rad2Deg;
        Quaternion _QuaternionTo = Quaternion.Euler(0.0f, _AngleInput + _AngleCamera, 0.0f);

        this.UpdateAsObservable()
            .Where(_ => !m_Transform.rotation.Equals(_QuaternionTo))
            .Subscribe(_ => {
                _TimeRotate += Time.deltaTime;
                float _Ratio = Mathf.Clamp01(_TimeRotate / TIME_ROTATE);

                m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, _QuaternionTo, _Ratio);
            })
        ;
    }

    override public void Move(Vector2 p_Move, Transform p_TransformCamera)
    {
        if (IsMoving())
        {
            m_RxMoved.OnNext(Unit.Default);
            return;
        }

        Vector3 _Direction = ConvDirection(p_Move);
        if (_Direction == Vector3.zero)
        {
            m_RxMoved.OnNext(Unit.Default);
            return;
        }

        Vector3 _Position = m_Transform.position;
        _Position.y = 1.0f;
        Ray _Ray = new Ray(_Position, _Direction);
        if (Physics.Raycast(_Ray, out RaycastHit _Hit, RATIO_MOVE + m_CharCtrl.radius))
        {
            // is hit
            m_RxMoved.OnNext(Unit.Default);
            return;
        }

        m_TimeMove = 0.0f;
        m_PositionMoveFrom = m_Transform.position;
        m_PositionMoveAdd = _Direction * RATIO_MOVE;

        m_ModelAnimationCtrl.Animation(Constant.ENUM_STATE_ANIME.STATE_ANIME_WALK);
    }

    public void OffCharaCtrl() { m_CharCtrl.enabled = false; }
    public bool IsMoving() { return (m_PositionMoveAdd != Vector3.zero); }

    override protected bool CanIdle() { return !IsMoving(); }
}
