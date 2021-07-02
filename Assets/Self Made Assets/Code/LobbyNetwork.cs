using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour
{
    [SerializeField] private string VersionName = "0.1";

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
        Debug.Log("Connecting to server...");
    }

    private void SetUsername()
    {
        PhotonNetwork.playerName = "Testing User";
    }

    private void OnConnectedToMaster()
    {
        SetUsername();
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected to master...");
    }

    private void OnJoinedLobby()
    {
        print("Joined lobby");
    }
}
