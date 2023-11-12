using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camcon : MonoBehaviour
{
    // Start is called before the first frame update
    public float zoomSpeed = 15f;
    public float minFOV = 15f;
    public float maxFOV = 90f;
    public float rotationSpeed = 100.0f; // Rotation speed
    private float currentRotationX = 0.0f; // Current X rotation of the camera

    // Maximum and minimum angles for rotation
    private float maxRotation = 30.0f;
    private float minRotation = 90.0f;
    public float speed = 5.0f;

    void Update()
    {
        // Existing zoom functionality
        if (Input.GetKey(KeyCode.C))
        {
            Camera.main.fieldOfView -= zoomSpeed * Time.deltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minFOV, maxFOV);
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            Camera.main.fieldOfView += zoomSpeed * Time.deltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minFOV, maxFOV);
        }

        // Existing vertical rotation functionality
        float verticalInput = Input.GetAxis("Vertical"); // Get vertical input (W and S keys)
        currentRotationX += verticalInput * rotationSpeed * Time.deltaTime;
        currentRotationX = Mathf.Clamp(currentRotationX, maxRotation, minRotation);
        transform.localEulerAngles = new Vector3(currentRotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);

        // New ghost-like floating camera movement with arrow keys
        float horizontalMovement = Input.GetAxis("Horizontal"); // Left/Right
        float verticalMovement = 0;

        Vector3 movementDirection = new Vector3(horizontalMovement, verticalMovement);
        movementDirection = transform.TransformDirection(movementDirection);
        transform.position += movementDirection * speed * Time.deltaTime;
    }

}
