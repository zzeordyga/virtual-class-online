using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private bool _isRotatable = false;
    public bool IsRotatable
    {
        get { return _isRotatable; }
        set { _isRotatable = value; }
    }
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;
    float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Locks the Cursor in the middle

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        // Gets the mouse input based on the sensitivity and adjusted with the frame rate

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation on X axis (increasing will result in flipping the rotation)
        xRotation -= mouseY;
        yRotation -= mouseX;


        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        //if (playerBody != null)
        //{
        //    if (_isRotatable)
        //    {
        //        yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        //        Debug.Log(yRotation);
        //    }
        //    transform.localRotation = Quaternion.Euler(0f, , 0f);
        //    Debug.Log(yRotation);
        //}

    }
}
