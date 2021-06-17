using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Awake()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        var random = Random.Range(30f, 30f);
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 1.2f, 15f), Quaternion.identity, 0);
        GameObject playerCamera = playerPrefab.transform.Find("CharacterCamera").gameObject;
        playerCamera.SetActive(false);
    }

}
