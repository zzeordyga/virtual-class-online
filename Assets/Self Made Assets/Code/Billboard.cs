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
        if (transform.parent.GetComponent<PhotonView>().isMine)
        {
            string username = PlayerNetwork.instance.PlayerInfo.UserName;
            string name = PlayerNetwork.instance.PlayerInfo.Name;
            if (Regex.Matches(username, @"[0-9]{10}").Count != 0)
            {
                playerName.text = username;
            } else
            {
                playerName.text = name;
            }
        }
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
