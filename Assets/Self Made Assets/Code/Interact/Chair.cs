using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Interactable
{
    public float min = 90f;
    public float max = 270f;

    private void Start()
    {
        max = gameObject.transform.localEulerAngles.y;
        min = gameObject.transform.localEulerAngles.y - 180f;
    }

    public override string GetDescription(GameObject player)
    {
        return "Press E to sit down";
    }

    public override void Interact(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();

        Debug.Log(player.transform.name);

        player.GetComponent<CharacterController>().enabled = false;

        player.transform.position = transform.Find("SitCheck").position - new Vector3(0f, 1.2f, 0f);


        if (!ReferenceEquals(playerAnimator, null)) playerAnimator.SetBool("Sitting", true);

        MouseLook ml = player.GetComponentInChildren<MouseLook>();
        if (!ReferenceEquals(ml, null))
        {
            Debug.Log("Please kepanggil");
            ml.ClampRotation(min, max);
        }
    }
}
