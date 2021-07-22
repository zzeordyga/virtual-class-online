using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    [SerializeField]
    private Player player;
    [SerializeField]
    private TMP_Text playerName;

    // Update is called once per frame

    void Start()
    {
        if (cam == null) cam = Camera.main.transform;
    }

    public void Init()
    {
        if (transform.parent.GetComponent<PhotonView>().isMine)
        {
            string name = PlayerNetwork.instance.PlayerInfo.Name;

            playerName.text = name;
        }

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            Debug.Log("[Billboard] Player Name : " + player.NickName + " | Player ID : " + player.ID + " | Current Player ID : " + this.player.currUid);

            if (this.player.currUid == player.ID)
            {
                Debug.Log("[Billboard] Changing the Text"); 
                playerName.text = player.NickName;
                break;
            }
        }
    }

    public void Init(int id)
    {
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            if(player.ID == id)
            {
                playerName.text = player.NickName;
            }
        }
        
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
