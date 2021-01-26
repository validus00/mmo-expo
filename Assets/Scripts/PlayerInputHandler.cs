using UnityEngine;

public class PlayerInputHandler : IPlayerInputHandler {
    public bool GetReturnKey() {
        return Input.GetKeyDown(KeyCode.Return);
    }

    public bool GetRightClickInputHeld() {
        return Input.GetButton(GameConstants.k_Fire2);
    }

    public Vector3 GetMoveInput() {
        float horizontalInput = Input.GetAxisRaw(GameConstants.k_Horizontal);
        float verticalInput = Input.GetAxisRaw(GameConstants.k_Vertical);

        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);
        // Prevents diagonal movement value from exceeding 1
        return Vector3.ClampMagnitude(move, 1);
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
