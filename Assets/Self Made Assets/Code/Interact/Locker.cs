using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : Interactable
{
    private bool isOpen = false;
    public override string GetDescription(GameObject player)
    {
        return "Press E to open locker";
    }

    public override void Interact(GameObject player)
    {
        transform.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player);
        GameObject lockerDoor = transform.Find("RotateAngle").gameObject;
        
        if (!isOpen)
        {
            lockerDoor.transform.Rotate(new Vector3(0f, -90f, 0f));
        } else
        {
            lockerDoor.transform.Rotate(new Vector3(0f, 90f, 0f));
        }

        isOpen = !isOpen;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Hello");
        if (stream.isWriting)
        {
            stream.SendNext(isOpen);
        }
        else
        {
            isOpen = (bool)stream.ReceiveNext();
        }
    }
}
