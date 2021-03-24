using System;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(EncountSceneConductor))]
public class EncountChecker : MonoBehaviour
{
    private readonly int TIMESPAN_POSITION = 1;
    private readonly float DISTANCE_RAY = 10.0f;

    [SerializeField] private Transform m_Character;
    private EncountSceneConductor m_SceneConductor;

    private Vector3 m_PositionBefore;
    [SerializeField, ReadOnly] private float m_SumMove = 0.0f;
    [SerializeField, ReadOnly] private int m_PerEncount = 0;
    private int m_JudgedUnit = 0;

    public float m_UnitMove;    // 2Dでいうところの1マスの距離
    public float m_NoEncount;   // エンカウントなしで移動可能な最低保証
    public float m_HighEncount; // エンカウントしないで移動できても、ある程度でエンカウントしてもらう
    public float m_PerHighEncount;

    private void Awake()
    {
        m_SceneConductor = GetComponent<EncountSceneConductor>();
        m_PositionBefore = m_Character.position;
    }

    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(TIMESPAN_POSITION))
            .Subscribe(_ => {
                Vector3 _PositionCurrent = m_Character.position;
                // 指定秒数以内に移動しても同一位置にいたらカウントされない
                m_SumMove += Vector3.Distance(_PositionCurrent, m_PositionBefore);
                m_PositionBefore = _PositionCurrent;

                if (m_SumMove < m_NoEncount)
                {
                    // 最低保証
                    return;
                }

                CheckEncount();
            })
            .AddTo(this)
        ;
    }

    private void CheckEncount()
    {
        float _Moved = m_SumMove - m_NoEncount;
        int _Unit = Mathf.CeilToInt(_Moved / m_UnitMove);
        if (_Unit <= m_JudgedUnit)
        {
            // 判定済み
            return;
        }

        Vector3 _Position = m_Character.position;
        _Position.y = 0.01f; // 少しだけ上に設定
        Ray _Ray = new Ray(_Position, -m_Character.up);
        foreach (RaycastHit _Hit in Physics.RaycastAll(_Ray, DISTANCE_RAY))
        {
            if (_Hit.transform.tag == Constant.TAG_LAND)
            {
                m_PerEncount = _Hit.transform.GetComponent<Land>().PerEncount; // 地形に設定されているエンカウント率を取得
                break;
            }
        }

        int _Per = m_PerEncount;
        if (m_HighEncount < m_SumMove)
        {
            // エンカウント率上昇
            _Per = Mathf.CeilToInt(_Per * m_PerHighEncount);
        }
        _Per = Mathf.Clamp(_Per, 0, 100);
        if (_Per <= 0)
        {
            return;
        }

        int _Random = (int)(UnityEngine.Random.value * 100.0f);
        if (_Per < _Random)
        {
            m_JudgedUnit = _Unit; // 判定済みユニットとして保持
        }
        else
        {
            // エンカウント
            m_SumMove = 0.0f;
            m_JudgedUnit = 0;
            m_SceneConductor.Encount();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 300, 20), "Sum move:" + m_SumMove.ToString());

        string _Str = "";
        if (m_SumMove < m_NoEncount)
        {
            _Str = " (No encount)";
        }
        else if(m_HighEncount < m_SumMove)
        {
            _Str = " (High encount x" + m_PerHighEncount.ToString() + ")";
        }
        GUI.Label(new Rect(20, 40, 300, 20), "Per encount:" + m_PerEncount.ToString() + "%" + _Str);
    }
}
