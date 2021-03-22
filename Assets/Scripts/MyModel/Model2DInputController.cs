using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model2DInputController : AbstractModelInputController
{
    override protected bool CanRotate(Vector2 p_Move, Vector3 p_ForwardCamera)
    {
        return true;
    }

    override public void Move(Vector2 p_Move, Transform p_TransformCamera)
    {

    }

    override protected bool CanIdle() { return true; }
}
