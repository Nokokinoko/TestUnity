using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSceneConductor : AbstractSceneConductor
{
  public override Constant.ENUM_SCENE IdScene { get { return Constant.ENUM_SCENE.SCENE_OBJECT_POOL; } }

  private void Start()
  {
    Started();
  }
}
