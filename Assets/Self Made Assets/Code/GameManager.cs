using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject SceneCamera;

    private void Awake()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        SceneCamera.SetActive(false);
    }

}
