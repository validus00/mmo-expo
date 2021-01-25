using UnityEngine;

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
    private Animator __animator;
    // For handling user movement inputs
    private IPlayerInputHandler __playerInputHandler;
    // Channel name input field
    private IInputFieldHandler __channelBoxHandler;
    // Chat input field
    private IInputFieldHandler __chatBoxHandler;
    // For handling disabling horizontal user movement during chat
    private bool __canMove = true;
    // For adding a little delay between chatting and horizontal user movement
    private int __delay = 0;
    // For setting up channelBoxHandler and chatBoxHandler
    private bool __isInputFieldInitialized = false;

    // For initializing playerInputHandler
    public void InitializePlayerInputHandler(IPlayerInputHandler playerInputHandler) {
        __playerInputHandler = playerInputHandler;
    }

    // For initalizing channelBoxHandler and chatBoxHandler
    public void InitializeInputFieldHandlers(IInputFieldHandler channelBoxHandler, IInputFieldHandler chatBoxHandler) {
        __channelBoxHandler = channelBoxHandler;
        __chatBoxHandler = chatBoxHandler;
        __isInputFieldInitialized = true;
    }

    // For setting up fpsCamera
    public void InitializeCamera(GameObject camera) {
        __fpsCamera = camera;
    }

    // Start is called before the first frame update
    void Start() {
        if (__playerInputHandler == null) {
            __playerInputHandler = new PlayerInputHandler();
        }

        if (!__isInputFieldInitialized) {
            __channelBoxHandler = GameObject.Find("ChannelInputField").GetComponent<InputFieldHandler>();
            __chatBoxHandler = GameObject.Find("MessageInputField").GetComponent<InputFieldHandler>();
            __isInputFieldInitialized = true;
        }
        __characterController = GetComponent<CharacterController>();
        __animator = GetComponent<Animator>();
        // Prevent User object from overlapping with another object
        __characterController.enableOverlapRecovery = true;
    }

    // Update is called once per frame
    void Update() {
        // Apply velocity only if user is allowed to move
        if (__canMove) {
            __HandleCharacterMovement();
        }
    }

    // For consistently periodic updates
    void FixedUpdate() {
        // If chat input field is selected, disable movement and apply a delay
        if (__chatBoxHandler.isFocused() || __channelBoxHandler.isFocused()) {
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

    private void __HandleCharacterMovement() {
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

        Vector3 move = __playerInputHandler.GetMoveInput();
        __animator.SetFloat("Horizontal", __GetAnimatorValue(move.x));
        __animator.SetFloat("Vertical", __GetAnimatorValue(move.z));

        Vector3 targetVelocity = __maxSpeedOnGround * transform.TransformVector(move);
        __characterVelocity = Vector3.Lerp(__characterVelocity, targetVelocity, __movementSharpnessOnGround * Time.deltaTime);
        __characterController.Move(__characterVelocity * Time.deltaTime);
    }

    private float __GetAnimatorValue(float input) {
        if (input > 0) {
            return 1f;
        } else if (0 > input) {
            return -1f;
        } else {
            return 0;
        }
    }
}
