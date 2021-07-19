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

public class GameManager : Photon.MonoBehaviour
{
    private class Token
    {
        public string token;
    }

    /// <summary>
    /// If we can, don't destroy the game manager
    /// </summary>
    /// 

    [Header("Scene Properties")]
    [SerializeField]
    private Camera videoCanvasCamera;
    [SerializeField]
    private VideoGroupController vgc;
    private String roomName = "";

    [Header("Agora Properties")]
    [SerializeField]
    private string AppID = "your_appid";
    [SerializeField]
    private Text appIDText;
    private IRtcEngine mRtcEngine = null;
    public IRtcEngine RtcEngine
    {
        get { return mRtcEngine; }
    }
    private string mChannel = "";
    private string tokenURL = "https://virtual-class-online.herokuapp.com/access_token?";
    private uint myUid;

    private static bool _initialized = false;

    [Header("Photon Properties")]
    [SerializeField]
    private string VersionName = "0.0.0";
    [SerializeField]
    private PhotonView PhotonView;

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    static IVideoChatClient app = null;

    public GameObject playerPrefab;

    private void Start()
    {
        CheckAppId(); //  Agora App Id
    }

    void Update()
    {
        CheckPermissions();
        CheckExit();
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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

        roomName = roomName.Substring(0, roomName.Length - 1);

        if (PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
        {
            Debug.Log("create room successfully sent");
            this.roomName = roomName;
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
            this.roomName = roomName;
        }
        else
        {
            Debug.Log("Failed to join");
        }
    }

    private bool isCreator = false;

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        Debug.Log("create room failed : " + codeAndMessage[1]);
    }

    private void OnCreatedRoom()
    {
        Debug.Log("Room create successfully");
        PhotonNetwork.LoadLevel("ClassScene");
        videoCanvasCamera.gameObject.SetActive(true);
        CreateAgoraChannel(roomName);
        isCreator = true;
    }

    private void OnJoinedRoom()
    {
        Debug.Log("Joined A Room Successfully (OnJoinedRoom)");
        videoCanvasCamera.gameObject.SetActive(true);
        if(!isCreator) JoinAgoraChannel(roomName);
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

    #region Agora Methods

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit, clean up...");
        if (!ReferenceEquals(app, null))
        {
            mRtcEngine.LeaveChannel();
            app.UnloadEngine();
        }
        IRtcEngine.Destroy();
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
    public void JoinAgoraChannel(string channelName)
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

        Debug.Log("Handling it perfectly");

        //app = new AgoraShareScreen();

        //if (app == null) return;

        LoadEngine(AppID);

        Join(channelName);
    }

    public void CreateAgoraChannel(string channelName)
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

        //app = new AgoraShareScreen();

        //if (app == null) return;

        LoadEngine(AppID);

        Create(channelName);
    }

    public void LoadEngine(string appId)
    {
        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        mRtcEngine.OnError = (code, msg) =>
        {
            Debug.LogErrorFormat("RTC Error:{0}, msg:{1}", code, IRtcEngine.GetErrorDescription(code));
        };

        mRtcEngine.OnWarning = (code, msg) =>
        {
            Debug.LogWarningFormat("RTC Warning:{0}, msg:{1}", code, IRtcEngine.GetErrorDescription(code));
        };

        // mRtcEngine.SetLogFile(logFilepath);
        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }
    
    public void Create(string channel)
    {

        if (mRtcEngine == null)
            return;

        mChannel = channel;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined = OnUserJoined;
        //mRtcEngine.OnUserOffline = OnUserOffline;
        //mRtcEngine.OnVideoSizeChanged = OnVideoSizeChanged;
        // Calling virtual setup function
        PrepareToJoin();

        // join channel (the real way)
        AgoraChannel newChannel = mRtcEngine.CreateChannel(mChannel);

        if (!ReferenceEquals(newChannel, null))
        {
            Debug.Log("Retrieving the token!");
            //GetAgoraToken(newChannel.ChannelId());
            GetAgoraToken(mChannel);
        }

        Debug.Log("initializeEngine done");
    }

    public void Join(string channel)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        mChannel = channel;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined = OnUserJoined;
        //mRtcEngine.OnUserOffline = OnUserOffline;
        //mRtcEngine.OnVideoSizeChanged = OnVideoSizeChanged;
        // Calling virtual setup function
        PrepareToJoin();

        // join channel (the real way)
        GetAgoraToken(mChannel);

        Debug.Log("initializeEngine done");
    }

    /// <summary>
    ///    Preparing video/audio/channel related characteric set up
    /// </summary>
    protected virtual void PrepareToJoin()
    {
        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        mRtcEngine.EnableAudio();
        // enable video
        mRtcEngine.EnableVideo();
        // allow camera output callback
        mRtcEngine.EnableVideoObserver();
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

    public void GetAgoraToken(string channelName)
    {
        StartCoroutine(GetToken(channelName));
    }

    IEnumerator GetToken(string channelName)
    {
        Debug.Log("Channel Name : " + channelName);
        //channelName = channelName.Substring(0, channelName.Length - 1);
        uint uid;
        //uid = Convert.ToUInt32(Math.Abs(channelName.GetHashCode()) % 1000) % 1000;
        uid = 0;


        Debug.Log(String.Format("{0}channel={1}&uid={2}", tokenURL, channelName, uid));

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(String.Format("{0}channel={1}&uid={2}", tokenURL, channelName, uid));
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + unityWebRequest.error);
        }
        else
        {
            Token tokenObject = JsonUtility.FromJson<Token>(unityWebRequest.downloadHandler.text);

            string token = tokenObject.token;

            Debug.Log("Joining result : " + mRtcEngine.JoinChannelByKey(token, channelName, "", 0));
            //mRtcEngine.EnableAudio();
            //mRtcEngine.EnableVideo();
            //mRtcEngine.EnableVideoObserver();
        }
    }

    #endregion

    #region Agora Engine Callbacks


    // implement engine callbacks
    protected virtual void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
        if (!PhotonView.isMine)
        {
            Debug.Log("What is mine");
            return;
        }
        else
        {
            Debug.Log("This is mine!");
        }

        myUid = uid;

        if (!ReferenceEquals(vgc, null))
        {
            Debug.Log("Adding a new video");
            vgc.AddVideo(uid, true);
            videoCanvasCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Video Controller not found");
        }
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    protected void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        if (!PhotonView.isMine)
        {
            Debug.Log("What is mine");
            return;
        }
        else
        {
            Debug.Log("This is mine!");
        }

        if (!ReferenceEquals(vgc, null))
        {
            Debug.Log("Adding a new video");
            vgc.AddVideo(uid, false);
        }
    }

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    protected virtual void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        //if (UserVideoDict.ContainsKey(uid))
        //{
        //    var surface = UserVideoDict[uid];
        //    surface.SetEnable(false);
        //    UserVideoDict.Remove(uid);
        //    GameObject.Destroy(surface.gameObject);
        //}
    }

    protected virtual void OnVideoSizeChanged(uint uid, int width, int height, int rotation)
    {
        Debug.LogWarningFormat("uid:{3} OnVideoSizeChanged width = {0} height = {1} for rotation:{2}", width, height, rotation, uid);

        //if (UserVideoDict.ContainsKey(uid))
        //{
        //    GameObject go = UserVideoDict[uid].gameObject;
        //    Vector2 v2 = new Vector2(width, height);
        //    RawImage image = go.GetComponent<RawImage>();
        //    //if (_enforcing360p)
        //    //{
        //    //    v2 = AgoraUIUtils.GetScaledDimension(width, height, 360f);
        //    //}

        //    if (IsPortraitOrientation(rotation))
        //    {
        //        v2 = new Vector2(v2.y, v2.x);
        //    }
        //    image.rectTransform.sizeDelta = v2;
        //}
    }

    bool IsPortraitOrientation(int rotation)
    {
        return rotation == 90 || rotation == 270;
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
