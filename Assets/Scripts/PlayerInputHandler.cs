using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {
    // Mouse look sensitivity
    [SerializeField]
    private float __lookSensitivity = 0.25f;
    private Animator __animator;

    void Start() {
        __animator = GetComponent<Animator>();
    }

    public bool GetRightClickInputHeld() {
        return Input.GetButton("Fire2");
    }

    public Vector3 GetMoveInput() {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        __animator.SetFloat("Horizontal", horizontalInput);
        __animator.SetFloat("Vertical", verticalInput);

        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);
        // Prevents diagonal movement value from exceeding 1
        return Vector3.ClampMagnitude(move, 1);
    }

    public float GetLookInputsHorizontal() {
        return Input.GetAxisRaw("Mouse X") * __GetPlatformSensitivityMultiplier() * 0.01f;
    }

    public float GetLookInputsVertical() {
        return Input.GetAxisRaw("Mouse Y") * __GetPlatformSensitivityMultiplier() * -0.01f;
    }

    private float __GetPlatformSensitivityMultiplier() {
#if UNITY_WEBGL
        Debug.Log("25");
        return 0.25f;
#else
        Debug.Log("1f");
        return 1f;
#endif
    }
}
