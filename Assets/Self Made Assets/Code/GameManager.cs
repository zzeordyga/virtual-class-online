using agora_gaming_rtc;
using agora_utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneEnum
{
    MainMenuScene,
    ClassScene
    // Add More Accordingly
};

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// If we can, don't destroy the game manager
    /// </summary>
    /// 

    [Header("Scene Properties")]
    [SerializeField]
    private Camera videoCanvasCamera;
    [SerializeField]
    private VideoGroupController vgc;
    private static String roomName = "";
    private Player currPlayer = null;
    private static List<uint> uidList = new List<uint>();
    public List<Player> players = new List<Player>();
    public Dictionary<Player, PhotonPlayer> photonPlayerDictionary = new Dictionary<Player, PhotonPlayer>();

    public static String RoomName
    {
        get { return roomName; }
    }

    [Header("Photon Properties")]
    [SerializeField]
    private string VersionName = "0.0.0";

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    public GameObject playerPrefab;

    private void Start()
    {
        /*CheckAppId();*/ //  Agora App Id
        
    }

    void Update()
    {
        //CheckPermissions();
        //CheckExit();
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    #region Photon Methods

    public void CreateRoom(string name)
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 5
        };

        roomName = name.Substring(0, name.Length - 1);

        if (PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
        {
            Debug.Log("create room successfully sent");
        }
        else
        {
            Debug.Log("create room failed");
        }
    }

    public void JoinRoom(string name)
    {
        roomName = name;

        if (PhotonNetwork.JoinRoom(name))
        {
            Debug.Log("Joined room successfully");
        }
        else
        {
            Debug.Log("Failed to join");
        }
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        


    }

    private bool isCreator = false;

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        Debug.Log("create room failed : " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        PhotonNetwork.player.NickName = PlayerNetwork.instance.PlayerInfo.UserName;
        Debug.Log("Room create successfully");
        PhotonNetwork.LoadLevel("ClassScene");
        videoCanvasCamera.gameObject.SetActive(true);
        //CreateAgoraChannel(roomName);
        //isCreator = true;
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.player.NickName = PlayerNetwork.instance.PlayerInfo.UserName;
        videoCanvasCamera.gameObject.SetActive(true);
        if (!isCreator)
        {
            //JoinAgoraChannel(roomName);
        }
    }

    // Done when user is joining a room
    public void SpawnPlayer()
    {
        var random = UnityEngine.Random.Range(30f, 30f);
        Transform[] allChildren = playerPrefab.GetComponentsInChildren<Transform>();
        foreach (Transform c in allChildren)
        {
            if (c.name.Equals("CharacterCamera"))
            {
                GameObject playerCamera = c.gameObject;
                playerCamera.SetActive(false);
            }
        }
        //playerPrefab.transform.Find("CharacterCamera").gameObject.SetActive(false);
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 1.2f, 15f), Quaternion.identity, 0);
    }

    #endregion

    #region Other Utilities
    public static uint GetInt64HashCode(string strText)
    {
        UInt32 hashCode = 0;
        if (!string.IsNullOrEmpty(strText))
        {
            //Unicode Encode Covering all characterset
            byte[] byteContents = Encoding.Unicode.GetBytes(strText);
            System.Security.Cryptography.SHA256 hash =
            new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] hashText = hash.ComputeHash(byteContents);
            //32Byte hashText separate
            //hashCodeStart = 0~7  8Byte
            //hashCodeMedium = 8~23  8Byte
            //hashCodeEnd = 24~31  8Byte
            //and Fold
            UInt32 hashCodeStart = BitConverter.ToUInt32(hashText, 0);
            UInt32 hashCodeMedium = BitConverter.ToUInt32(hashText, 8);
            UInt32 hashCodeEnd = BitConverter.ToUInt32(hashText, 24);
            hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        }
        return (hashCode);
    }

    #endregion


    
}
