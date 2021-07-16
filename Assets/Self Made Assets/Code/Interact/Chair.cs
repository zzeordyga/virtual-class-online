using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Interactable
{
    public override void Interact()
    {
        Debug.Log("Sitting");
    }
}
