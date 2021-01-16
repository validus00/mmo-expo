using UnityEngine;

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

    private Rigidbody __rb;
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
        // Assign to player Rigidbody in order to move it
        __rb = GetComponent<Rigidbody>();
        __velocity = Vector3.zero;
        __rotation = Vector3.zero;
        __cameraUpDownRotation = 0f;
        __currentCameraUpAndDownRotation = 0f;
    }

    // Update is called once per frame
    void Update() {
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        // Calculate movement as a 3D vector
        Vector3 movementHorizontal = transform.right * xMovement;
        Vector3 movementVertical = transform.forward * zMovement;
        Vector3 movementVelocity = (movementHorizontal + movementVertical).normalized * __speed;

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

        // Apply movement
        __Move(movementVelocity);

        // Apply rotation
        Vector3 rotationVector = new Vector3(0, yRotation, 0) * __lookSensitivity;
        __Rotate(rotationVector);

        // Apply horizontal mouse free look
        __RotateCamera(cameraUpAndDownRotation);
    }

    // For updates when using Rigidbody
    void FixedUpdate() {
        if (__velocity != Vector3.zero) {
            __rb.MovePosition(__rb.position + __velocity * Time.fixedDeltaTime);
        }

        __rb.MoveRotation(__rb.rotation * Quaternion.Euler(__rotation));

        if (__fpsCamera != null) {
            __currentCameraUpAndDownRotation -= __cameraUpDownRotation;
            __currentCameraUpAndDownRotation = Mathf.Clamp(__currentCameraUpAndDownRotation, -85, 85);
            __fpsCamera.transform.localEulerAngles = new Vector3(__currentCameraUpAndDownRotation, 0, 0);
        }
    }

    private void __Move(Vector3 movementVelocity) {
        __velocity = movementVelocity;
    }

    private void __Rotate(Vector3 rotationVector) {
        __rotation = rotationVector;
    }

    private void __RotateCamera(float cameraUpAndDownRotation) {
        __cameraUpDownRotation = cameraUpAndDownRotation;
    }
}
