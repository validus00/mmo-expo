using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    // Player movement speed
    [SerializeField]
    private float speed = 10f;

    // Mouse look sensitivity
    [SerializeField]
    private float lookSensitivity = 3f;

    // FPS camera
    [SerializeField]
    private GameObject fpsCamera;

    private Rigidbody rb;
    // velocity = Player movement horizontal velocity
    private Vector3 velocity = Vector3.zero;
    // rotation = Player rotation
    private Vector3 rotation = Vector3.zero;
    // cameraUpDownRotation = FPS camera horizontal free look input
    private float cameraUpDownRotation = 0f;
    // currentCameraUpAndDownRotation = current FPS camera horizontal position
    private float currentCameraUpAndDownRotation = 0f;

    // Start is called before the first frame update
    void Start() {
        // Assign to player Rigidbody in order to move it
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        // Calculate movement as a 3D vector
        Vector3 movementHorizontal = transform.right * xMovement;
        Vector3 movementVertical = transform.forward * zMovement;
        Vector3 movementVelocity = (movementHorizontal + movementVertical).normalized * speed;

        float yRotation = 0f;
        float cameraUpAndDownRotation = 0f;

        // If right mouse button is held down, then hide the mouse cursor and allow mouse free look
        if (Input.GetButton("Fire2")) {
            Cursor.visible = false;

            // Calculate rotation as a 3D vector for turning around
            yRotation = Input.GetAxis("Mouse X");

            // Calculate look up and down camera rotation
            cameraUpAndDownRotation = Input.GetAxis("Mouse Y") * lookSensitivity;
        } else {
            // Otherwise, show the mouse cursory on screen
            Cursor.visible = true;
        }

        // Apply movement
        Move(movementVelocity);

        // Apply rotation
        Vector3 rotationVector = new Vector3(0, yRotation, 0) * lookSensitivity;
        Rotate(rotationVector);

        // Apply horizontal mouse free look
        RotateCamera(cameraUpAndDownRotation);
    }

    // For updates when using Rigidbody
    void FixedUpdate() {
        if (velocity != Vector3.zero) {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        if (fpsCamera != null) {
            currentCameraUpAndDownRotation -= cameraUpDownRotation;
            currentCameraUpAndDownRotation = Mathf.Clamp(currentCameraUpAndDownRotation, -85, 85);
            fpsCamera.transform.localEulerAngles = new Vector3(currentCameraUpAndDownRotation, 0, 0);
        }
    }

    private void Move(Vector3 movementVelocity) {
        velocity = movementVelocity;
    }

    private void Rotate(Vector3 rotationVector) {
        rotation = rotationVector;
    }

    private void RotateCamera(float cameraUpAndDownRotation) {
        cameraUpDownRotation = cameraUpAndDownRotation;
    }
}
