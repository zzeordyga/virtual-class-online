using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainLobbyController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _roomName;

    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 5
        };

        if (PhotonNetwork.CreateRoom(_roomName.text, roomOptions, TypedLobby.Default))
        {
            Debug.Log("create room successfully sent");
        }
        else
        {
            Debug.Log("create room failed");
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        Debug.Log("create room failed : " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        Debug.Log("Room create successfully");
    }
}
