using UnityEngine;

public class PlayerInputHandler : IPlayerInputHandler
{
    public bool GetTabKey()
    {
        return Input.GetKeyDown(KeyCode.Tab);
    }

    public bool GetReturnKey()
    {
        return Input.GetKeyDown(KeyCode.Return);
    }

    public bool GetRightClickInputHeld()
    {
        return Input.GetMouseButton(1);
    }

    public Vector3 GetMoveInput()
    {
        float horizontalInput = GetAInput() + GetDInput();
        float verticalInput = GetSInput() + GetWInput();

        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);
        // Prevents diagonal movement value from exceeding 1
        return Vector3.ClampMagnitude(move, 1);
    }

    public Vector3 GetFlyingInput(Vector3 vector)
    {
        float horizontalInput = GetAInput() + GetDInput();
        float verticalInput = GetSInput() + GetWInput();

        float yRaw = GetSpaceInput() + GetXInput();
        float y = (yRaw != 0) ? yRaw : verticalInput * vector.y;

        Vector3 move = new Vector3(horizontalInput, y, verticalInput * vector.z);
        // Prevents diagonal movement value from exceeding 1
        return Vector3.ClampMagnitude(move, 1);
    }

    private float GetSpaceInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            return 1f;
        }
        return 0;
    }

    private float GetXInput()
    {
        if (Input.GetKey(KeyCode.X))
        {
            return -1f;
        }
        return 0;
    }

    private float GetWInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            return 1f;
        }
        return 0;
    }

    private float GetAInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            return -1f;
        }
        return 0;
    }

    private float GetSInput()
    {
        if (Input.GetKey(KeyCode.S))
        {
            return -1f;
        }
        return 0;
    }

    private float GetDInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return 1f;
        }
        return 0;
    }

    public float GetLookInputsHorizontal()
    {
        return Input.GetAxisRaw(GameConstants.K_MouseX) * GetLookSensitivityMultiplier() * 0.01f;
    }

    public float GetLookInputsVertical()
    {
        return Input.GetAxisRaw(GameConstants.K_MouseY) * GetLookSensitivityMultiplier() * -0.01f;
    }

    private float GetLookSensitivityMultiplier()
    {
#if UNITY_WEBGL
        return 0.25f;
#else
        return 1f;
#endif
    }
}
