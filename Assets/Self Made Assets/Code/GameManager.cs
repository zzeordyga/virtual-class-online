using agora_gaming_rtc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// If we can, don't destroy the game manager
    /// </summary>
    /// 
    [Header("Agora Properties")]
    [SerializeField]
    private string AppID = "your_appid";
    private string TokenID = "your_token";

    [Header("Photon Properties")]
    [SerializeField]
    private string VersionName = "0.0.0";

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    static IVideoChatClient app = null;
    public static IRtcEngine rtcEngine = null;

    public GameObject playerPrefab;

    //private void Start()
    //{
    //    CheckAppId();
    //    LoadLastChannel();
    //}

    //void Update()
    //{
    //    CheckPermissions();
    //    CheckExit();
    //}

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        PhotonNetwork.ConnectUsingSettings(VersionName);
        //SpawnPlayer();
    }

    public static void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 5
        };

        if (PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
        {
            Debug.Log("create room successfully sent");
        }
        else
        {
            Debug.Log("create room failed");
        }
    }

    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }


    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        Debug.Log("create room failed : " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        Debug.Log("Length of Rooms : " + PhotonNetwork.GetRoomList().Length);
        Debug.Log("Room create successfully");
        PhotonNetwork.LoadLevel(1);
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("ClassScene");
        SpawnPlayer();
    }

    // Done when user is joining a room
    public void SpawnPlayer()
    {
        var random = Random.Range(30f, 30f);
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 1.2f, 15f), Quaternion.identity, 0);
        GameObject playerCamera = playerPrefab.transform.Find("CharacterCamera").gameObject;
        playerCamera.SetActive(false);
    }

    #region Agora Methods

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit, clean up...");
        if (!ReferenceEquals(app, null))
        {
            app.UnloadEngine();
        }
        IRtcEngine.Destroy();
    }

    private void LoadLastChannel()
    {
        string channel = PlayerPrefs.GetString("ChannelName");
        if (!string.IsNullOrEmpty(channel))
        {
            GameObject go = GameObject.Find("ChannelName");
            // TODO : Create or Join room when clicking a room list

        }
    }

    private void CheckAppId()
    {
        Debug.Assert(AppID.Length > 10, "AppId is problematic");
    }

    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {                 
				Permission.RequestUserPermission(permission);
			}
        }
#endif
    }

    void CheckExit()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Gracefully quit on OS like Android, so OnApplicationQuit() is called
            Application.Quit();
#endif
        }
    }
    void CheckDevices(IRtcEngine engine)
    {
        VideoDeviceManager deviceManager = VideoDeviceManager.GetInstance(engine);
        deviceManager.CreateAVideoDeviceManager();

        int cnt = deviceManager.GetVideoDeviceCount();
        Debug.Log("Device count =============== " + cnt);
    }

    #endregion

}
