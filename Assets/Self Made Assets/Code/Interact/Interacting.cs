using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacting : MonoBehaviour
{
    private float range = 15f;
    void Update()
    {
        Camera playerCamera = transform.Find("CharacterCamera").gameObject.GetComponent<Camera>();
        RaycastHit hit;
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * range, Color.green);

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            var parent = hit.collider.transform.parent;
            if (parent != null)
            {
                Interactable interactableObject = parent.gameObject.GetComponent<Interactable>();
                if (interactableObject != null)
                {
                    interactableObject.Action();
                }
            }
        }
    }
}
