using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private float interactionDistance = 15f;
    [SerializeField] private MouseLook mouseLook;

    public TextMeshProUGUI interactionDescription;

    private Animator playerAnimator;
    private Camera playerCamera;

    private void Start()
    {
        playerAnimator = transform.GetComponent<Animator>();
    }

    void Update()
    {

        Player playerMovement = transform.GetComponent<Player>();
        if (playerAnimator.GetBool("Sitting"))
        {
            playerMovement.enabled = false;
            mouseLook.IsRotatable = true;
        } else
        {
            playerMovement.enabled = true;
            mouseLook.IsRotatable = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerAnimator.GetBool("Sitting")) {
                playerAnimator.SetBool("Sitting", false);
            }
        }

        Transform[] allChildren = transform.GetComponentsInChildren<Transform>();
        foreach (Transform c in allChildren)
        {
            if (c.name.Equals("CharacterCamera"))
            {
                playerCamera = c.gameObject.GetComponent<Camera>();
            }
        }
        //playerCamera = transform.Find("CharacterCamera").GetComponent<Camera>();
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance , Color.green);
        bool successHit = false;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionDistance))
        {
            var parent = hit.collider.transform.parent;
            if (parent != null)
            {
                Interactable interactableObject = parent.gameObject.GetComponent<Interactable>();
                if (interactableObject != null)
                {
                    HandlerInteraction(interactableObject);
                    interactionDescription.text = interactableObject.GetDescription(transform.gameObject);
                    successHit = true;
                }
            }
        }
        if (!successHit)
        {
            interactionDescription.text = "";
        }
    }

    public void HandlerInteraction(Interactable interactableObject)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactableObject.Interact(transform.gameObject);
        }
    }
}
