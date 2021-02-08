using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UniRx;

public class SingletonSceneLoader : SingletonMonoBehaviour<SingletonSceneLoader>
{
  [SerializeField, ReadOnly]
  private Constant.ENUM_SCENE m_IdSceneActive;
  public Constant.ENUM_SCENE IdSceneActive { set { m_IdSceneActive = value; } }

  private List<AsyncOperation> m_ListAsyncOperation = new List<AsyncOperation>();
  private List<Constant.ENUM_SCENE> m_ListLoadScene = new List<Constant.ENUM_SCENE>();

  private BridgeSceneConductor m_BridgeSceneConductor = null;
  private BridgeSceneConductor BridgeSceneConductor
  {
    get
    {
      if (m_BridgeSceneConductor == null)
      {
        Assert.IsTrue(IsContainScene(Constant.ENUM_SCENE.SCENE_BRIDGE), "BridgeScene is not contains");
        m_BridgeSceneConductor = (BridgeSceneConductor)GetSceneConductor(Constant.ENUM_SCENE.SCENE_BRIDGE);
      }
      return m_BridgeSceneConductor;
    }
  }

  private void InitListScene()
  {
    m_ListAsyncOperation.Clear();
    m_ListLoadScene.Clear();
  }

  public bool IsActive(Constant.ENUM_SCENE pIdScene)
  {
    return (m_IdSceneActive == pIdScene);
  }

  private string GetNameSceneById(Constant.ENUM_SCENE pIdScene)
  {
    string _Name = "";
    switch (pIdScene)
    {
      case Constant.ENUM_SCENE.SCENE_BRIDGE:
        _Name = "BridgeScene";
        break;
      case Constant.ENUM_SCENE.SCENE_GAME:
        _Name = "GameScene";
        break;
      case Constant.ENUM_SCENE.SCENE_OBJECT_POOL:
        _Name = "ObjectPoolScene";
        break;
    }
    return _Name;
  }

  private bool IsContainScene(Constant.ENUM_SCENE pIdScene)
  {
    return SceneManager.GetSceneByName(GetNameSceneById(pIdScene)).IsValid();
  }

  #region GET SCENE CONDUCTOR
  private AbstractSceneConductor GetSceneConductor(Constant.ENUM_SCENE pIdScene)
  {
    Scene _Scene = SceneManager.GetSceneByName(GetNameSceneById(pIdScene));
    return GetSceneConductor(_Scene);
  }

  private AbstractSceneConductor GetSceneConductor(Scene pScene)
  {
    AbstractSceneConductor _Conductor = null;
    foreach (GameObject _Obj in pScene.GetRootGameObjects())
    {
      _Conductor = _Obj.GetComponent<AbstractSceneConductor>();
      if (_Conductor != null)
      {
        break;
      }
    }
    return _Conductor;
  }
  #endregion

  #region GO TO SCENE
  public void GoToGame()
  {
    InitListScene();

    m_ListLoadScene.Add(Constant.ENUM_SCENE.SCENE_BRIDGE);
    m_ListLoadScene.Add(Constant.ENUM_SCENE.SCENE_GAME);

    ProcessScene(true);
  }

  public void GoToObjectPool()
  {
    InitListScene();

    m_ListLoadScene.Add(Constant.ENUM_SCENE.SCENE_BRIDGE);
    m_ListLoadScene.Add(Constant.ENUM_SCENE.SCENE_OBJECT_POOL);

    ProcessScene(true);
  }
  #endregion

  #region LOAD / UNLOAD
  public void PreloadScene(Constant.ENUM_SCENE pIdScene)
  {
    InitListScene();

    m_ListLoadScene.Add(pIdScene);
    m_ListLoadScene.Add(Constant.ENUM_SCENE.SCENE_BRIDGE);

    ProcessScene(false);
  }

  private void ProcessScene(bool pFade)
  {
    Action _Action = () =>
    {
      AsyncOperation _Async = null;
      for (int _Idx = 0; _Idx < SceneManager.sceneCount; _Idx++)
      {
        Scene _Scene = SceneManager.GetSceneAt(_Idx);
        AbstractSceneConductor _Conductor = GetSceneConductor(_Scene);

        if (m_ListLoadScene.Contains(_Conductor.IdScene))
        {
          // already load
          m_ListLoadScene.Remove(_Conductor.IdScene);
        }
        else
        {
          // unload
          _Async = SceneManager.UnloadSceneAsync(_Scene.name);
          m_ListAsyncOperation.Add(_Async);
        }
      }

      // load scene
      foreach (Constant.ENUM_SCENE _IdScene in m_ListLoadScene)
      {
        _Async = SceneManager.LoadSceneAsync(GetNameSceneById(_IdScene), LoadSceneMode.Additive);
        m_ListAsyncOperation.Add(_Async);
      }
      // m_ListLoadSceneには実際にLoadSceneAsyncを実行した要素のみ保持

      StartCoroutine(CoroutineLoad(pFade));
    };

    if (pFade)
    {
      BridgeSceneConductor.Fader.RxCompletedFadeIn
        .First()
        .Subscribe(_ => _Action())
      ;
      BridgeSceneConductor.Fader.FadeIn();
    }
    else
    {
      _Action();
    }
  }

  private IEnumerator CoroutineLoad(bool pFade)
  {
    foreach (AsyncOperation _Async in m_ListAsyncOperation)
    {
      while (!_Async.isDone)
      {
        yield return null;
      }
    }
    // all async operation is done

    for (int _Idx = 0; _Idx < SceneManager.sceneCount; _Idx++)
    {
      Scene _Scene = SceneManager.GetSceneAt(_Idx);
      AbstractSceneConductor _Conductor = GetSceneConductor(_Scene);
      while (!_Conductor.ImReady)
      {
        yield return null;
      }
    }
    // all scene conductor is ready

    if (pFade)
    {
      BridgeSceneConductor.Fader.FadeOut();
    }
  }
  #endregion
}
