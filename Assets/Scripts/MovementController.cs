using Photon.Pun;
using UnityEngine;

/*
 * MovementController class is for implementing user movement controls
 */
public class MovementController : MonoBehaviour
{
    // Maximum player move speed value
    private const float K_MaxSpeedOnGround = 20f;
    // Player acceleration and deceleration value
    private const float K_MovementSharpnessOnGround = 15f;
    // Camera rotation speed value
    private const float K_RotationSpeed = 200f;
    // Player velocity vector
    private Vector3 _characterVelocity;
    // Camera vertical angle
    private float _cameraVerticalAngle = 0f;
    // FPS camera
    public GameObject FpsCamera;
    private CharacterController _characterController;
    private Animator _animator;
    // For handling user movement inputs
    public IPlayerInputHandler PlayerInputHandler;
    // Channel name input field
    public IInputFieldHandler ChannelBoxHandler;
    // Chat input field
    public IInputFieldHandler ChatBoxHandler;
    // For handling panel related logic
    public IPanelManager PanelManager;
    public PhotonView PhotonView;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerInputHandler == null)
        {
            PlayerInputHandler = new PlayerInputHandler();
        }
        if (ChannelBoxHandler == null)
        {
            ChannelBoxHandler = GameObject.Find(GameConstants.K_ChannelInputField).GetComponent<InputFieldHandler>();
        }
        if (ChatBoxHandler == null)
        {
            ChatBoxHandler = GameObject.Find(GameConstants.K_MessageInputField).GetComponent<InputFieldHandler>();
        }
        if (PanelManager == null)
        {
            PanelManager = GameObject.Find(GameConstants.K_PanelManager).GetComponent<PanelManager>();
        }
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        // Prevent User object from overlapping with another object
        _characterController.enableOverlapRecovery = true;
    }

    // Update is called once per frame 
    void Update()
    {
        // Toggle exit event panel as active or inactive
        if (PlayerInputHandler.GetTabKey())
        {
            // Show and unlock mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PanelManager.ToggleExitEventPanel();
        }
        // Handle user and camera movement only when panels aren't active
        if (!PanelManager.IsAnyPanelActive())
        {
            HandleCharacterMovement();
        }
    }

    private void HandleCharacterMovement()
    {
        // If right mouse button is held down, then hide the mouse cursor and allow mouse free look
        if (PlayerInputHandler.GetRightClickInputHeld())
        {
            // Hide and lock mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Rotate user horizontally
            transform.Rotate(new Vector3(0f, (PlayerInputHandler.GetLookInputsHorizontal() * K_RotationSpeed), 0f), Space.Self);
            // Rotate user camera vertically
            _cameraVerticalAngle += PlayerInputHandler.GetLookInputsVertical() * K_RotationSpeed;
            // Limit vertical camera rotation angle of up to +/- 89 degrees
            _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);
            FpsCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
        }
        else
        {
            // Show and unlock mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        Vector3 move;
        // Apply velocity only if user is allowed to move
        if (!ChatBoxHandler.IsFocused() && !ChannelBoxHandler.IsFocused())
        {
            move = PlayerInputHandler.GetMoveInput();
        }
        else
        {
            move = Vector3.zero;
        }
        // If any movement detected, enable character controller
        if (_characterController.enabled == false && (move.x != 0 || move.y != 0 || move.z != 0))
        {
            Debug.Log("character velocity @@");
            _characterController.enabled = true;
            if (PhotonView != null)
            {
                PhotonView.RPC("CharacterControllerToggle", RpcTarget.AllBuffered, _characterController.enabled);
            }
        }
        // Set float values for animation controller
        _animator.SetFloat(GameConstants.K_Horizontal, GetAnimatorValue(move.x));
        _animator.SetFloat(GameConstants.K_Vertical, GetAnimatorValue(move.z));

        if (_characterController.enabled == true)
        {
            // Apply velocity to User object
            Vector3 targetVelocity = K_MaxSpeedOnGround * transform.TransformVector(move);
            _characterVelocity = Vector3.Lerp(_characterVelocity, targetVelocity, K_MovementSharpnessOnGround * Time.deltaTime);
            _characterController.Move(_characterVelocity * Time.deltaTime);
        }
    }

    private float GetAnimatorValue(float input)
    {
        if (input > 0)
        {
            return 1f;
        }
        if (0 > input)
        {
            return -1f;
        }
        return 0;
    }
}
