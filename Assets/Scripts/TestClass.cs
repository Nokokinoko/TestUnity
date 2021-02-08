// warning CS0162: Unreachable code detected
#pragma warning disable 162
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;
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
    StartCoroutine(BeCalledCoroutine());
  }

  private IEnumerator BeCalledCoroutine()
  {
    UnityEngine.Debug.Log("be called coroutine");
    yield return null;

    UnityEngine.Debug.Log("next frame");
    yield return new WaitForSeconds(1.0f);

    UnityEngine.Debug.Log("after 1sec");
    yield break;

    UnityEngine.Debug.Log("finish (do not call)");
  }
  #endregion

  #region CALL ASYNC
  // using System.Threading.Tasks
  private void CallAsync()
  {
    AsyncMethod();
    AsyncSerial().ContinueWith(_ => AsyncSerial()); // 直列
  }

  private async void AsyncMethod()
  {
    UnityEngine.Debug.Log("async method start");
    await Task.Delay(1000);
    UnityEngine.Debug.Log("async method end");
  }

  private async Task AsyncSerial()
  {
    UnityEngine.Debug.Log("async serial start");
    await Task.Delay(1000);
    UnityEngine.Debug.Log("async serial end");
  }
  #endregion
}
