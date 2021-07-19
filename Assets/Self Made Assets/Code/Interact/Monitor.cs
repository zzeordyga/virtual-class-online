using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : Interactable
{
    private bool isOn = false;

    GameObject player = null;

    public override string GetDescription(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator.GetBool("Sitting") == false)
        {
            return "";
        }
        return "Press E to use PC";
    }

    public override void Interact(GameObject player)
    {
        this.player = player;
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator.GetBool("Sitting"))
        {
            MouseLook ml = player.GetComponentInChildren<MouseLook>();
            GameObject ScreenUI = transform.Find("UI").gameObject;
            if (!isOn)
            {
                ScreenUI.SetActive(true);
                if (!ReferenceEquals(ml, null))
                {
                    GetComponentInChildren<AgoraShareScreen>().SetupUI();
                    ml.Lock();
                    ml.EnableCursor();
                }
            } else
            {
                ScreenUI.SetActive(false);
                if (!ReferenceEquals(ml, null))
                {
                    GetComponentInChildren<AgoraShareScreen>().StopScreenCapture();
                    ml.Unlock();
                    ml.DisableCursor();
                }
            }
            isOn = !isOn;
        }
    }

    public void Interact()
    {
        if (player == null)
            return;

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator.GetBool("Sitting"))
        {
            MouseLook ml = player.GetComponentInChildren<MouseLook>();
            GameObject ScreenUI = transform.Find("UI").gameObject;
            if (!isOn)
            {
                ScreenUI.SetActive(true);
                if (!ReferenceEquals(ml, null))
                {
                    GetComponentInChildren<AgoraShareScreen>().SetupUI();
                    ml.Lock();
                    ml.EnableCursor();
                }
            }
            else
            {
                ScreenUI.SetActive(false);
                if (!ReferenceEquals(ml, null))
                {
                    ml.Unlock();
                    ml.DisableCursor();
                }
            }
            isOn = !isOn;
        }
    }

}
