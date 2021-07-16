using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Action()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Action();
        }
    }

    public virtual void Interact()
    {
        Debug.Log(transform.gameObject.name);
    }
}
