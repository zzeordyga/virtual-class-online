using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public CharacterController controller;
    public GameObject playerCamera;
    public Animator anim;

    #region Movement Variables
    public float speed = 12f;
    public bool isRunning = false;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    #endregion

    #region Ground Checking Utils
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    #endregion

    #region Everchanging Variables
    Vector3 velocity;
    bool isGrounded;
    #endregion

    private void Awake()
    {
        if (photonView.isMine)
        {
            playerCamera.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Checks using a made sphere with a radius if a certain layer is hit (in this case, Ground Layer)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Resets the speed of our fall
        if(isGrounded && velocity.y < 0)
        {
            // Not zero so that we don't immediately reset speed upon colliding with ground
            velocity.y = -2f;
        }

        if (photonView.isMine)
        {
            CheckMovement();
        }
    }

    private void CheckMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftShift)) isRunning = true;
        else isRunning = false;

        if (isRunning) speed = 18f;
        else speed = 12f;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Formula for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y = gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

}
