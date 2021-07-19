using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{
    [SerializeField]
    private RoomGroupViewController _roomViewGroup;
    private RoomGroupViewController RoomViewGroup
    {
        get { return _roomViewGroup; }
    }

    public void JoinRoom(string roomName)
    {
        GameManager gm = FindObjectOfType<GameManager>();

        Debug.Log("Lobby canvas name : " + roomName);

        if (!ReferenceEquals(gm, null)) gm.JoinRoom(roomName);
    }

}
