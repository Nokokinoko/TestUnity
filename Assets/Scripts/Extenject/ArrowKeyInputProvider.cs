using UnityEngine;

public class ArrowKeyInputProvider : IInputProvider
{
    public bool Up()
    {
        return Input.GetKey(KeyCode.UpArrow);
    }

    public bool Right()
    {
        return Input.GetKey(KeyCode.RightArrow);
    }

    public bool Down()
    {
        return Input.GetKey(KeyCode.DownArrow);
    }

    public bool Left()
    {
        return Input.GetKey(KeyCode.LeftArrow);
    }
}
