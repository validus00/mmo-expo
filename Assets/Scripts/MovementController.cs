using UnityEngine;
using UnityEngine.UI;

/*
 * MovementController class is for implementing user movement controls
 */
public class MovementController : MonoBehaviour {
    // Player movement speed
    [SerializeField]
    private float __speed = 10f;

    // Mouse look sensitivity
    [SerializeField]
    private float __lookSensitivity = 2.5f;

    // FPS camera
    [SerializeField]
    private GameObject __fpsCamera;
    
    // Chat input field
    private InputField __chatBox;
    // For handling disabling horizontal user movement during chat
    private bool __canMove;
    // For adding a little delay between chatting and horizontal user movement
    private int __delay;
    // For accessing the rigidbody to move it
    private Rigidbody __rigidbody;
    // velocity = Player movement horizontal velocity
    private Vector3 __velocity;
    // rotation = Player rotation
    private Vector3 __rotation;
    // cameraUpDownRotation = FPS camera horizontal free look input
    private float __cameraUpDownRotation;
    // currentCameraUpAndDownRotation = current FPS camera horizontal position
    private float __currentCameraUpAndDownRotation;

    // Start is called before the first frame update
    void Start() {
        __rigidbody = GetComponent<Rigidbody>();
        __chatBox = GameObject.Find("MessageInputField").GetComponent<InputField>();
        __canMove = true;
        __delay = 0;
        __velocity = Vector3.zero;
        __rotation = Vector3.zero;
        __cameraUpDownRotation = 0f;
        __currentCameraUpAndDownRotation = 0f;
    }

    // Update is called once per frame
    void Update() {
        // Apply velocity only if user is allowed to move
        if (__canMove) {
            // Get user inputs
            float xMovement = Input.GetAxis("Horizontal");
            float zMovement = Input.GetAxis("Vertical");

            // Calculate movement as a 3D vector
            Vector3 movementHorizontal = transform.right * xMovement;
            Vector3 movementVertical = transform.forward * zMovement;
            Vector3 movementVelocity = (movementHorizontal + movementVertical).normalized * __speed;

            // Apply movement
            __Move(movementVelocity);
        } else {
            __Move(Vector3.zero);
        }

        float yRotation = 0f;
        float cameraUpAndDownRotation = 0f;

        // If right mouse button is held down, then hide the mouse cursor and allow mouse free look
        if (Input.GetButton("Fire2")) {
            Cursor.visible = false;

            // Calculate rotation as a 3D vector for turning around
            yRotation = Input.GetAxis("Mouse X");

            // Calculate look up and down camera rotation
            cameraUpAndDownRotation = Input.GetAxis("Mouse Y") * __lookSensitivity;
        } else {
            // Otherwise, show the mouse cursory on screen
            Cursor.visible = true;
        }

        // Apply rotation
        Vector3 rotationVector = new Vector3(0, yRotation, 0) * __lookSensitivity;
        __Rotate(rotationVector);

        // Apply horizontal mouse free look
        __RotateCamera(cameraUpAndDownRotation);
    }

    // For updates when using Rigidbody
    void FixedUpdate() {
        // If chat input field is selected, disable movement and apply a delay
        if (__chatBox.isFocused) {
            __delay = 20;
            __canMove = false;
        } else if (__delay > 0) {
            __delay--;
        }

        // Allow movement if no delay
        if (0 >= __delay) {
            __canMove = true;
        }

        // Apply new horizontal position
        if (__velocity != Vector3.zero) {
            __rigidbody.MovePosition(__rigidbody.position + __velocity * Time.fixedDeltaTime);
        }

        // Apply new horizontal rotation
        __rigidbody.MoveRotation(__rigidbody.rotation * Quaternion.Euler(__rotation));

        // Apply vertical camera rotation
        if (__fpsCamera != null) {
            __currentCameraUpAndDownRotation -= __cameraUpDownRotation;
            // Set a max +/- 85 rotational view from midpoint of the screen
            __currentCameraUpAndDownRotation = Mathf.Clamp(__currentCameraUpAndDownRotation, -85, 85);
            __fpsCamera.transform.localEulerAngles = new Vector3(__currentCameraUpAndDownRotation, 0, 0);
        }
    }

    // Helper function for updating horizontal velocity value
    private void __Move(Vector3 movementVelocity) {
        __velocity = movementVelocity;
    }

    // Helper function for updating horizontal rotation value
    private void __Rotate(Vector3 rotationVector) {
        __rotation = rotationVector;
    }

    // Helper function for updating vertical camera rotation value
    private void __RotateCamera(float cameraUpAndDownRotation) {
        __cameraUpDownRotation = cameraUpAndDownRotation;
    }
}
