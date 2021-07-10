using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainLobbyController : MonoBehaviour
{
    public MenuController menuController;
    [SerializeField]
    private TextMeshProUGUI _roomName;

    public void OnClick_CreateRoom()
    {
        GameManager.CreateRoom(_roomName.text);
    }

    private void OnCreatedRoom()
    {
        Debug.Log("Length of Rooms : " + PhotonNetwork.GetRoomList().Length);
        Debug.Log("Room create successfully");
        PhotonNetwork.LoadLevel(1);
    }

    public void LogOut()
    {
        PhotonNetwork.Disconnect();
        PlayerNetwork.instance.PlayerInfo = null;
        menuController.SwitchMenu(1);
    }

}
