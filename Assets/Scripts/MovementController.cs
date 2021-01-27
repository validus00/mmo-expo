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
    public GameObject fpsCamera;
    private CharacterController __characterController;
    private Animator __animator;
    // For handling user movement inputs
    public IPlayerInputHandler playerInputHandler;
    // Channel name input field
    public IInputFieldHandler channelBoxHandler;
    // Chat input field
    public IInputFieldHandler chatBoxHandler;
    // For handling panel related logic
    public IPanelManager panelManager;
    // For adding a little delay between chatting and horizontal user movement
    private int __delay = 0;

    // Start is called before the first frame update
    void Start() {
        if (playerInputHandler == null) {
            playerInputHandler = new PlayerInputHandler();
        }
        if (channelBoxHandler == null) {
            channelBoxHandler = GameObject.Find(GameConstants.k_ChannelInputField).GetComponent<InputFieldHandler>();
        }
        if (chatBoxHandler == null) {
            chatBoxHandler = GameObject.Find(GameConstants.k_MessageInputField).GetComponent<InputFieldHandler>();
        }
        if (panelManager == null) {
            panelManager = GameObject.Find(GameConstants.k_PanelManager).GetComponent<PanelManager>();
        }
        __characterController = GetComponent<CharacterController>();
        __animator = GetComponent<Animator>();
        // Prevent User object from overlapping with another object
        __characterController.enableOverlapRecovery = true;
    }

    // Update is called once per frame 
    void Update() {
        // Toggle exit event panel as active or inactive
        if (playerInputHandler.GetTabKey()) {
            // Show and unlock mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            panelManager.ToggleExitEventPanel();
        }
        // Handle user and camera movement only when panels aren't active
        if (!panelManager.IsAnyPanelActive()) {
            __HandleCharacterMovement();
        }
    }

    private void __HandleCharacterMovement() {
        // If right mouse button is held down, then hide the mouse cursor and allow mouse free look
        if (playerInputHandler.GetRightClickInputHeld()) {
            // Hide and lock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Rotate user horizontally
            transform.Rotate(new Vector3(0f, (playerInputHandler.GetLookInputsHorizontal() * __rotationSpeed), 0f), Space.Self);
            // Rotate user camera vertically
            __cameraVerticalAngle += playerInputHandler.GetLookInputsVertical() * __rotationSpeed;
            // Limit vertical camera rotation angle of up to +/- 89 degrees
            __cameraVerticalAngle = Mathf.Clamp(__cameraVerticalAngle, -89f, 89f);
            fpsCamera.transform.localEulerAngles = new Vector3(__cameraVerticalAngle, 0, 0);
        } else {
            // Show and unlock mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        Vector3 move;
        // Apply velocity only if user is allowed to move
        if (!chatBoxHandler.isFocused() && !channelBoxHandler.isFocused()) {
            move = playerInputHandler.GetMoveInput();
        } else {
            move = Vector3.zero;
        }
        // Set float values for animation controller
        __animator.SetFloat(GameConstants.k_Horizontal, __GetAnimatorValue(move.x));
        __animator.SetFloat(GameConstants.k_Vertical, __GetAnimatorValue(move.z));
        // Apply velocity to User object
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


