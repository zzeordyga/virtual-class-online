using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour
{
    public MenuController menuController;
    [SerializeField] private static string VersionName = "0.0";

    private void Start()
    {
       
    }

    public static void Init()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
        Debug.Log("Connecting to server...");
    }

    private void SetUserProperties()
    {
        PlayerInfo _playerInfo = PlayerNetwork.instance.PlayerInfo;
        PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
            { "BinusianId", _playerInfo.BinusianId},
            { "Major", _playerInfo.Major},
            { "Name", _playerInfo.Name},
            { "PictureId", _playerInfo.PictureId},
            { "Role", _playerInfo.Role},
            { "UserId", _playerInfo.UserId},
            { "UserName", _playerInfo.UserName},
        });
    }

    private void OnConnectedToMaster()
    {
        SetUserProperties();
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected to master...");
        Debug.Log("Connected as " + PhotonNetwork.player.CustomProperties["UserName"]);
    }

    private void OnJoinedLobby()
    {
        menuController.SwitchMenu(3);
    }
}
