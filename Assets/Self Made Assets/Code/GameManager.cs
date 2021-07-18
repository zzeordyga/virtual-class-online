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
    private class Token
    {
        public string token;
    }

    private static uint PLAYER_UID = 0;

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
    private IRtcEngine mRtcEngine = null;
    private string mChannel = "";
    private string tokenURL = "https://virtual-class-online.herokuapp.com/access_token?";
    protected Dictionary<uint, VideoSurface> UserVideoDict = new Dictionary<uint, VideoSurface>();
    protected const string SelfVideoName = "MyView";

    private static bool _initialized = false;

    [Header("Photon Properties")]
    [SerializeField]
    private string VersionName = "0.0.0";

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    static IVideoChatClient app = null;

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
        Debug.Log("Masuk on Joined Room");
        //PhotonNetwork.LoadLevel("ClassScene");
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

        Debug.Log("Handling it perfectly");

        //app = new AgoraShareScreen();

        //if (app == null) return;

        LoadEngine(AppID);

        Join(channelName);

        //app.OnViewControllerFinish += OnViewControllerFinish;
        //// load engine
        //app.LoadEngine(AppID);
        //// join channel and jump to next scene
        //app.Join(channelName);
        //SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        //SceneManager.LoadScene(SceneEnum.ClassScene.ToString(), LoadSceneMode.Single);

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

    public void Join(string channel)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        mChannel = channel;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        //mRtcEngine.OnUserJoined = OnUserJoined;
        //mRtcEngine.OnUserOffline = OnUserOffline;
        //mRtcEngine.OnVideoSizeChanged = OnVideoSizeChanged;
        // Calling virtual setup function
        PrepareToJoin();

        // join channel by key (sample)
        //mRtcEngine.JoinChannelByKey("00669c9d55ce62e4a3384886a8b1de3261dIACiuB5WpMn+Vq/Tt0KxU17RWGm1/NZw7CAV09wQLtqJzDLRTXgAAAAAEACqPfBqvn/jYAEAAQC+f+Ng", channel, "", 0);

        // join channel (the real way)
        if (mRtcEngine.JoinChannel(mChannel, null, 0) != 0)
        {
            Debug.Log("Mau bikin channel nich, namanya : " + mChannel);

            AgoraChannel newChannel = mRtcEngine.CreateChannel(mChannel);

            if (!ReferenceEquals(newChannel, null)) GetAgoraToken(newChannel);
            else Debug.Log("Anjing null ternyata");
        }

        Debug.Log("initializeEngine done");
    }

    /// <summary>
    ///    Preparing video/audio/channel related characteric set up
    /// </summary>
    protected virtual void PrepareToJoin()
    {
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

    public void GetAgoraToken(AgoraChannel newChannel)
    {
        StartCoroutine(GetToken(newChannel));
    }

    IEnumerator GetToken(AgoraChannel channel)
    {
        uint uid = 0;

        string channelName = channel.ChannelId().Substring(0, channel.ChannelId().Length-1);
        Debug.Log("Channel name : " + channelName);
        Debug.Log("Channel name length : " + channelName.Length);

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

            Debug.Log("Token : " + token);
            mRtcEngine.JoinChannelByKey(token, channelName, "", 0);
        }
    }

    #endregion

    #region Agora Engine Callbacks
    // implement engine callbacks
    protected virtual void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    protected virtual void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(30);
            videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);
            UserVideoDict[uid] = videoSurface;
            Vector2 pos = AgoraUIUtils.GetRandomPosition(100);
            videoSurface.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        }
    }

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    protected virtual void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        if (UserVideoDict.ContainsKey(uid))
        {
            var surface = UserVideoDict[uid];
            surface.SetEnable(false);
            UserVideoDict.Remove(uid);
            GameObject.Destroy(surface.gameObject);
        }
    }

    protected virtual void OnVideoSizeChanged(uint uid, int width, int height, int rotation)
    {
        Debug.LogWarningFormat("uid:{3} OnVideoSizeChanged width = {0} height = {1} for rotation:{2}", width, height, rotation, uid);

        if (UserVideoDict.ContainsKey(uid))
        {
            GameObject go = UserVideoDict[uid].gameObject;
            Vector2 v2 = new Vector2(width, height);
            RawImage image = go.GetComponent<RawImage>();
            //if (_enforcing360p)
            //{
            //    v2 = AgoraUIUtils.GetScaledDimension(width, height, 360f);
            //}

            if (IsPortraitOrientation(rotation))
            {
                v2 = new Vector2(v2.y, v2.x);
            }
            image.rectTransform.sizeDelta = v2;
        }
    }

    bool IsPortraitOrientation(int rotation)
    {
        return rotation == 90 || rotation == 270;
    }


    protected VideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;

        // to be renderered onto
        RawImage image = go.AddComponent<RawImage>();
        image.rectTransform.sizeDelta = new Vector2(1, 1);// make it almost invisible

        // make the object draggable
        go.AddComponent<UIElementDragger>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            go.transform.SetParent(canvas.transform);
        }
        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        Vector2 v2 = AgoraUIUtils.GetRandomPosition(100);
        go.transform.localPosition = new Vector3(v2.x, v2.y, 0);
        go.transform.localScale = Vector3.one;

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }
    #endregion

}
