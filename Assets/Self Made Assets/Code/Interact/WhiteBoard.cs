using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBoard : Interactable
{
    public override string GetDescription(GameObject player)
    {
        return "Press E to draw on whiteboard";
    }

    public override void Interact(GameObject player)
    {
        
    }
}
