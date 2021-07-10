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
        GameManager.JoinRoom(roomName);
    }

}
