using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Interactable
{
    public override string GetDescription()
    {
        return "Press E to sit down";
    }

    public override void Interact(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        playerAnimator.SetBool("Sitting", true);
    }
}
