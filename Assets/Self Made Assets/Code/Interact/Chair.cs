using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Interactable
{
    public float min = 90;
    public float max = 270;

    public override string GetDescription()
    {
        return "Press E to sit down";
    }

    public override void Interact(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();

        player.transform.position = transform.Find("SitCheck").position - new Vector3(0f, 5f, 0f);
        if(!ReferenceEquals(playerAnimator, null)) playerAnimator.SetBool("Sitting", true);

        MouseLook ml = player.GetComponent<MouseLook>();
        if (!ReferenceEquals(ml, null)) ml.ClampRotation(min, max);
    }
}
