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

        Camera playerCamera = transform.Find("CharacterCamera").gameObject.GetComponent<Camera>();
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionDistance))
        {
            bool successHit = false;
            var parent = hit.collider.transform.parent;
            if (parent != null)
            {
                Interactable interactableObject = parent.gameObject.GetComponent<Interactable>();
                if (interactableObject != null)
                {
                    HandlerInteraction(interactableObject);
                    interactionDescription.text = interactableObject.GetDescription();
                    successHit = true;
                }
            }
            if (!successHit)
            {
                interactionDescription.text = "";
            }
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
