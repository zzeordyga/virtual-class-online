using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : Interactable
{
    private bool isOn = false;
    public override string GetDescription(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        Debug.Log(playerAnimator.GetBool("Sitting"));
        if (playerAnimator.GetBool("Sitting") == false)
        {
            return "";
        }
        return "Press E to use PC";
    }

    public override void Interact(GameObject player)
    {
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
                    ml.Lock();
                    ml.EnableCursor();
                }
            } else
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
