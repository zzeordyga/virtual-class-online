using agora_gaming_rtc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [Header("Agora Properties")]
    [SerializeField]
    private string AppID = "your_appid";
    [SerializeField]
    private string TokenID = "your_token";
    [SerializeField]
    private Text appIDText;

    private static bool _initialized = false;

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

    private void Start()
    {
        CheckAppId(); //  Agora App Id
        //LoadLastChannel(); //  Load a default channel for each player (Not Needed)
    }

    void Update()
    {
        CheckPermissions();
        CheckExit();
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        //PhotonNetwork.ConnectUsingSettings(VersionName);
        //SpawnPlayer();
    }

    #region Photon Methods

    public void CreateRoom(string roomName)
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
            // TODO : Create channel using backend Node.js (or this might work! hopefully!!)
            HandleSceneChange(roomName);
        }
        else
        {
            Debug.Log("create room failed");
        }
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            Debug.Log("Joined room successfully");
            HandleSceneChange(roomName);
        }
        
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        Debug.Log("create room failed : " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        Debug.Log("Length of Rooms : " + PhotonNetwork.GetRoomList().Length);
        Debug.Log("Room create successfully");
        PhotonNetwork.LoadLevel("ClassScene");
    }

    private void OnJoinedRoom()
    {
        //PhotonNetwork.LoadLevel("ClassScene");
    }

    // Done when user is joining a room
    public void SpawnPlayer()
    {
        var random = Random.Range(30f, 30f);
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
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 1.2f, 15f), Quaternion.identity, 0);
    }

    #endregion

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

    // This is not needed because we are going to always make users choose the channel
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
        if (AppID.Length > 10)
        {
            SetAppIdText();
            _initialized = true;
        }
    }

    void SetAppIdText()
    {
        if (appIDText != null)
            appIDText.text = "AppID:" + AppID.Substring(0, 4) + "********" + AppID.Substring(AppID.Length - 4, 4);
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

    // Call when successfully logged in
    public void HandleSceneChange(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            Debug.LogError("Channel name can not be empty!");
            return;
        }

        if (!_initialized)
        {
            Debug.LogError("AppID null or app is not initialized properly!");
            return;
        }

        app = new AgoraShareScreen();

        if (app == null) return;

        app.OnViewControllerFinish += OnViewControllerFinish;
        // load engine
        app.LoadEngine(AppID);
        // join channel and jump to next scene
        app.Join(channelName);
        //SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        //SceneManager.LoadScene(SceneEnum.ClassScene.ToString(), LoadSceneMode.Single);

    }

    public void OnViewControllerFinish()
    {
        if (!ReferenceEquals(app, null))
        {
            app = null; // delete app
            LeaveShareScreen();
            //SceneManager.LoadScene(SceneEnum.MainMenuScene.ToString(), LoadSceneMode.Single); 
        }
    }

    public void LeaveShareScreen()
    {
        // TODO : Do something when player is out of share screen mode
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (!ReferenceEquals(app, null))
        {
            app.OnSceneLoaded(); // call this after scene is loaded
        }

        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    #endregion

}
