using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeDraw;

public class WhiteBoard : Interactable
{ 
    public Camera playerCam;
    public Camera whiteboardCam;
    private int isOn = 0;
    public int IsOn{
        get{return isOn;}
        set{isOn = value;}
    }
    public override string GetDescription(GameObject player)
    {
        if (isOn != 0)
        {
            return "";
        }
        return "Press E to draw on whiteboard";
    }

    public override void Interact(GameObject player)
    {
        transform.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
        Animator playerAnimator = player.GetComponent<Animator>();
        MouseLook ml = player.GetComponentInChildren<MouseLook>();
        GameObject ScreenUI = transform.Find("UI").gameObject;
        if (isOn == 0)
        {
            ScreenUI.SetActive(true);
            if (!ReferenceEquals(ml, null))
            {
                ml.Lock();
                ml.EnableCursor();
            }
            player.GetComponent<CharacterController>().enabled = false;
            isOn = 1;
        } else
        {
            ScreenUI.SetActive(false);
            if (!ReferenceEquals(ml, null))
            {
                ml.Unlock();
                ml.DisableCursor();
            }
            player.GetComponent<CharacterController>().enabled = true;
            isOn = 2;
        }
        // isOn = !isOn;
        playerCam.enabled = !playerCam.enabled;
        whiteboardCam.enabled = !whiteboardCam.enabled;
    }
}
