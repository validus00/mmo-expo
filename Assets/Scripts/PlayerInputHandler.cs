using UnityEngine;

public class PlayerInputHandler : IPlayerInputHandler {
    private float __lookSensitivity = 1f;

    public bool GetRightClickInputHeld() {
        return Input.GetButton("Fire2");
    }

    public Vector3 GetMoveInput() {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);
        // Prevents diagonal movement value from exceeding 1
        return Vector3.ClampMagnitude(move, 1);
    }

    public float GetLookInputsHorizontal() {
        return Input.GetAxisRaw("Mouse X") * __GetLookSensitivityMultiplier() * __lookSensitivity * 0.01f;
    }

    public float GetLookInputsVertical() {
        return Input.GetAxisRaw("Mouse Y") * __GetLookSensitivityMultiplier() * __lookSensitivity * -0.01f;
    }

    private float __GetLookSensitivityMultiplier() {
#if UNITY_WEBGL
        return 0.25f;
#else
        return 1f;
#endif
    }
}
