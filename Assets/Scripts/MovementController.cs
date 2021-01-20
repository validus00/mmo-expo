using UnityEngine;
using UnityEngine.UI;

/*
 * MovementController class is for implementing user movement controls
 */
public class MovementController : MonoBehaviour {
    // Maximum player move speed value
    [SerializeField]
    private float __maxSpeedOnGround = 20f;
    // Player acceleration and deceleration value
    private readonly float __movementSharpnessOnGround = 15f;
    // Camera rotation speed value
    private readonly float __rotationSpeed = 200f;
    // Player velocity vector
    private Vector3 __characterVelocity;
    // Camera vertical angle
    private float __cameraVerticalAngle = 0f;
    // FPS camera
    [SerializeField]
    private GameObject __fpsCamera;
    private CharacterController __characterController;
    // For handling user movement inputs
    private PlayerInputHandler __playerInputHandler;
    // Channel name input field
    private InputField __channelBox;
    // Chat input field
    private InputField __chatBox;
    // For handling disabling horizontal user movement during chat
    private bool __canMove = true;
    // For adding a little delay between chatting and horizontal user movement
    private int __delay = 0;

    // Start is called before the first frame update
    void Start() {
        __characterController = GetComponent<CharacterController>();
        __playerInputHandler = GetComponent<PlayerInputHandler>();
        __channelBox = GameObject.Find("ChannelInputField").GetComponent<InputField>();
        __chatBox = GameObject.Find("MessageInputField").GetComponent<InputField>();
        // Prevent User object from overlapping with another object
        __characterController.enableOverlapRecovery = true;
    }

    // Update is called once per frame
    void Update() {
        // Apply velocity only if user is allowed to move
        if (__canMove) {
            HandleCharacterMovement();
        }
    }

    // For consistently periodic updates
    void FixedUpdate() {
        // If chat input field is selected, disable movement and apply a delay
        if (__chatBox.isFocused || __channelBox.isFocused) {
            __delay = 20;
            __canMove = false;
        } else if (__delay > 0) {
            __delay--;
        }
        // Allow movement if no delay
        if (0 >= __delay) {
            __canMove = true;
        }
    }

    private void HandleCharacterMovement() {
        // If right mouse button is held down, then hide the mouse cursor and allow mouse free look
        if (__playerInputHandler.GetRightClickInputHeld()) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Rotate user horizontally
            transform.Rotate(new Vector3(0f, (__playerInputHandler.GetLookInputsHorizontal() * __rotationSpeed), 0f), Space.Self);
            // Rotate user camera vertically
            __cameraVerticalAngle += __playerInputHandler.GetLookInputsVertical() * __rotationSpeed;
            // Limit vertical camera rotation angle of up to +/- 89 degrees
            __cameraVerticalAngle = Mathf.Clamp(__cameraVerticalAngle, -89f, 89f);
            __fpsCamera.transform.localEulerAngles = new Vector3(__cameraVerticalAngle, 0, 0);
        } else {
            // Otherwise, show the mouse cursory on screen
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        // Apply velocity to User object
        Vector3 targetVelocity = __maxSpeedOnGround * transform.TransformVector(__playerInputHandler.GetMoveInput());
        __characterVelocity = Vector3.Lerp(__characterVelocity, targetVelocity, __movementSharpnessOnGround * Time.deltaTime);
        __characterController.Move(__characterVelocity * Time.deltaTime);
    }
}
