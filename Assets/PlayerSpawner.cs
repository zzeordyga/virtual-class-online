using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Awake()
    {
        
        GameManager gm = FindObjectOfType<GameManager>();
        if(!ReferenceEquals(gm, null))
        {
            gm.SpawnPlayer();
        }
    }
}
