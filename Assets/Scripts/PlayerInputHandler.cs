using UnityEngine;

public class PlayerInputHandler : IPlayerInputHandler {
    public bool GetTabKey() {
        return Input.GetKeyDown(KeyCode.Tab);
    }

    public bool GetReturnKey() {
        return Input.GetKeyDown(KeyCode.Return);
    }

    public bool GetRightClickInputHeld() {
        return Input.GetMouseButton(1);
    }

    public Vector3 GetMoveInput() {
        float horizontalInput = __GetAInput() + __GetDInput();
        float verticalInput = __GetSInput() + __GetWInput();

        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);
        // Prevents diagonal movement value from exceeding 1
        return Vector3.ClampMagnitude(move, 1);
    }

    private float __GetWInput() {
        if (Input.GetKey(KeyCode.W)) {
            return 1f;
        }
        return 0;
    }

    private float __GetAInput() {
        if (Input.GetKey(KeyCode.A)) {
            return -1f;
        }
        return 0;
    }

    private float __GetSInput() {
        if (Input.GetKey(KeyCode.S)) {
            return -1f;
        }
        return 0;
    }

    private float __GetDInput() {
        if (Input.GetKey(KeyCode.D)) {
            return 1f;
        }
        return 0;
    }

    public float GetLookInputsHorizontal() {
        return Input.GetAxisRaw(GameConstants.k_MouseX) * __GetLookSensitivityMultiplier() * 0.01f;
    }

    public float GetLookInputsVertical() {
        return Input.GetAxisRaw(GameConstants.k_MouseY) * __GetLookSensitivityMultiplier() * -0.01f;
    }

    private float __GetLookSensitivityMultiplier() {
#if UNITY_WEBGL
        return 0.25f;
#else
        return 1f;
#endif
    }
}
