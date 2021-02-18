using UnityEngine;
using UniRx;

public class ObjectPoolSceneConductor : AbstractSceneConductor
{
  public override Constant.ENUM_SCENE IdScene { get { return Constant.ENUM_SCENE.SCENE_OBJECT_POOL; } }

  private readonly string NAME_BUTTON_GO_TO = "ButtonGoTo";

  private void Start()
  {
    Transform _ObjButton = Canvas.transform.Find(NAME_BUTTON_GO_TO);
    if (_ObjButton == null)
    {
      Debug.Log("require " + NAME_BUTTON_GO_TO + " gameobject");
      return;
    }

    MyButton _Button = _ObjButton.GetComponent<MyButton>();
    if (_Button == null)
    {
      Debug.Log("require MyButton component (" + NAME_BUTTON_GO_TO + ")");
      return;
    }

    _Button.RxOnClick
      .First()
      .Subscribe(_ => SingletonSceneLoader.Instance.GoToGame())
      .AddTo(this)
    ;

    Started();
  }
}
