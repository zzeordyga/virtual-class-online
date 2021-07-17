using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : Interactable
{
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
          //logic here
        }
    }
}
