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

    private float _minRotation = 0f;
    private float _maxRotation = 360f;

    public float MinRotation
    {
        get { return _minRotation; }
        set { _minRotation = value; }
    }

    public float MaxRotation
    {
        get { return _maxRotation; }
        set { _maxRotation = value; }
    }

    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;
    float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Locks the Cursor in the middle

        DisableCursor();
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
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (yRotation >= 360 || yRotation <= -360) yRotation = 0;

        if (playerBody != null)
        {
            if (_isRotatable)
            {
                yRotation = Mathf.Clamp(yRotation, MinRotation, MaxRotation);
                
            }
            playerBody.localRotation = Quaternion.Euler(0f, -yRotation, 0f);
        }

    }

    public void ClampRotation(float min, float max)
    {
        MinRotation = min;
        MaxRotation = max;

    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Lock()
    {
        this.enabled = false;
    }

    public void Unlock()
    {
        this.enabled = true;
    }
}
