using System.Collections;
using System.Collections.Generic;
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
        //playerName.text = PhotonNetwork.player.CustomProperties["Name"].ToString();
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
