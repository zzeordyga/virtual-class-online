using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    private PhotonView PhotonView;
    public GameObject playerCamera;
    public CharacterController controller;
    private Animator playerAnimator;
    private Vector3 move;
    private Quaternion rotate;

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
        PhotonView = GetComponent<PhotonView>();
        if (PhotonView.isMine)
        {
            playerCamera.SetActive(true);
            playerAnimator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonView.isMine)
        {
            Cleaning();
            CheckMovement();
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Hello");
        //if (stream.isWriting)
        //{
        //    stream.SendNext(transform.position);
        //    Debug.Log("send" + transform.position);
        //    //stream.SendNext(transform.rotation);
        //}
        //else
        //{
        //    move = (Vector3)stream.ReceiveNext();
        //    Debug.Log("Receive" + move);
        //}
    }

    private void Cleaning()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            playerAnimator.SetBool("Cleaning", true);
        } else
        {
            playerAnimator.SetBool("Cleaning", false);
        }
        
    }

    private void CheckMovement()
    {
        // Checks using a made sphere with a radius if a certain layer is hit (in this case, Ground Layer)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Resets the speed of our fall
        if (isGrounded && velocity.y < 0)
        {
            // Not zero so that we don't immediately reset speed upon colliding with ground
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        playerAnimator.SetFloat("InputX", z);
        playerAnimator.SetFloat("InputY", x);

        move = transform.right * x + transform.forward * z;

        //if (Input.GetKey(KeyCode.LeftShift)) isRunning = true;
        //else isRunning = false;

        //if (isRunning) speed = 18f;
        //else speed = 12f;

        controller.Move(move * speed * Time.deltaTime);

        //if (Input.GetButtonDown("Jump") && isGrounded)
        //{
        //    // Formula for jumping
        //    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        //}

        velocity.y = gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if(transform.position.y > 2.0f)
        {
            Debug.Log("HERE IT IS");
            transform.position = new Vector3(transform.position.x, 1.08f, transform.position.z);
        }
    }

}
