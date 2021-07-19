using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightButton : Interactable
{
    private bool isOn = false;
    public Light _light;
    public override string GetDescription(GameObject player)
    {
        return "Press E to toggle light";
    }

    public override void Interact(GameObject player)
    {
        transform.GetComponent<PhotonView>().RequestOwnership();
        _light.enabled = !isOn;
        isOn = !isOn;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(isOn);
        }
        else
        {
            _light.enabled = (bool)stream.ReceiveNext();
        }
    }
}
