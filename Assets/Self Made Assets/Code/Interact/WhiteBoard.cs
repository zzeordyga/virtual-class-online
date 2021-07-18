using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBoard : Interactable
{ 
    public Camera playerCam;
    public Camera whiteboardCam;
    private bool isOn = false;
    public override string GetDescription(GameObject player)
    {
        if (isOn)
        {
            return "";
        }
        return "Press E to draw on whiteboard";
    }

    public override void Interact(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        MouseLook ml = player.GetComponentInChildren<MouseLook>();
        GameObject ScreenUI = transform.Find("UI").gameObject;
        if (!isOn)
        {
            ScreenUI.SetActive(true);
            if (!ReferenceEquals(ml, null))
            {
                ml.Lock();
                ml.EnableCursor();
            }
            player.GetComponent<CharacterController>().enabled = false;
        } else
        {
            ScreenUI.SetActive(false);
            if (!ReferenceEquals(ml, null))
            {
                ml.Unlock();
                ml.DisableCursor();
            }
            player.GetComponent<CharacterController>().enabled = true;
        }
        isOn = !isOn;
        playerCam.enabled = !playerCam.enabled;
        whiteboardCam.enabled = !whiteboardCam.enabled;
    }
}
