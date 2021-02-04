using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class TestClass : MonoBehaviour
{
  #region MEASURE TIME
  // using System.Diagnostics
  private void MeasureTime()
  {
    Stopwatch _Stopwatch = new Stopwatch();
    _Stopwatch.Start();
    // 計測する処理
    _Stopwatch.Stop();
    UnityEngine.Debug.Log(_Stopwatch.Elapsed);
  }
  #endregion

  #region CALL COROUTINE
  // using System.Collections
  private void CallCoroutine()
  {
    StartCoroutine(BeCalled());
  }

  private IEnumerator BeCalled()
  {
    UnityEngine.Debug.Log("be called");
    yield return null;

    UnityEngine.Debug.Log("next frame");
    yield return new WaitForSeconds(1.0f);

    UnityEngine.Debug.Log("after 1sec");
    yield break;

    UnityEngine.Debug.Log("finish (do not call)");
  }
  #endregion
}
