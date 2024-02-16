using UnityEngine;

public class WasdKeyInputProvider : IInputProvider
{
    public bool Up()
    {
        return Input.GetKey(KeyCode.W);
    }

    public bool Right()
    {
        return Input.GetKey(KeyCode.D);
    }

    public bool Down()
    {
        return Input.GetKey(KeyCode.S);
    }

    public bool Left()
    {
        return Input.GetKey(KeyCode.A);
    }
}
