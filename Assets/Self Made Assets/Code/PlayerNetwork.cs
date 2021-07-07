using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour
{
    public static PlayerNetwork instance;
    [SerializeField]
    private PlayerInfo _playerInfo;
    public PlayerInfo PlayerInfo
    {
        get { return _playerInfo; } 
        set { _playerInfo = value; }
    }
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
}
