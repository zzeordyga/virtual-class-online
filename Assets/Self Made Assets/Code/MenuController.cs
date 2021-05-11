using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject RoomPanel;

    [SerializeField] private InputField roomInput;

    // The string that holds the name of the room chosen
    private string currRoom;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
    }

    private void Start()
    {
        
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    private void SetUsername()
    {
        // This is for setting the display name for the user, not for the Room name
        PhotonNetwork.playerName = roomInput.text;
    }

    public void CreateRoom()
    {
        string roomName = roomInput.text;

        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { maxPlayers = 10 }, null);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(currRoom);
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("ClassScene");
    }

    private void SwitchMenu(int flag)
    {
        if(flag == 1)
        {
            MainPanel.SetActive(false);
            RoomPanel.SetActive(true);
        }
        else
        {
            MainPanel.SetActive(true);
            RoomPanel.SetActive(false);
        }
    }

}
