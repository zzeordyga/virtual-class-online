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
        GameManager.CreateRoom(_roomName.text);
    }

}
