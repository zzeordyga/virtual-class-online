using agora_gaming_rtc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{
    private class Token
    {
        public string token;
    }

    private PhotonView PhotonView;
    public GameObject playerCamera;
    public CharacterController controller;
    private Animator playerAnimator;
    private Vector3 move;
    private Quaternion rotate;
    private List<GameObject> playerVideoList;

    #region Movement Variables
    public float speed = 12f;
    public bool isRunning = false;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    #endregion

    #region Ground Checking Utils
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    #endregion

    #region Everchanging Variables
    Vector3 velocity;
    bool isGrounded;
    #endregion

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

    private static uint myUid = 9032;
    public static uint MyUiD
    {
        get { return myUid; }
    }
    public string MChannel
    {
        get { return mChannel; }
    }

    private bool isJoined = false;

    public Dictionary<uint, bool> UIDDictionary = new Dictionary<uint, bool>();

    private void Start()
    {
        CheckAppId();
        PhotonView = GetComponent<PhotonView>();

        if (!photonView.isMine)
        {
            return;
        }

        playerCamera.SetActive(true);
        playerAnimator = GetComponent<Animator>();

        LoadEngine(AppID);

        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccessHandler;
        mRtcEngine.OnUserJoined = OnUserJoinedHandler;
        mRtcEngine.OnLeaveChannel = OnLeaveChannelHandler;
        mRtcEngine.OnUserOffline = OnUserOfflineHandler;

        PrepareToJoin();

        Debug.Log("Player is awake");
        playerVideoList = new List<GameObject>();
        JoinAgoraChannel(GameManager.RoomName);
    }

    // Update is called once per frame
    void Update()
    {
        CheckPermissions();
        CheckExit();

        if (PhotonView.isMine)
        {
            Cleaning();
            CheckMovement();
        }
    }

    public void CreateUserVideoSurface(uint uid, bool isLocalUser)
    {
        // Avoid duplicating Local player VideoSurface image plane.
        for (int i = 0; i < playerVideoList.Count; i++)
        {
            if (playerVideoList[i].name == uid.ToString())
            {
                return;
            }
        }

        VideoGroupController vgc = FindObjectOfType<VideoGroupController>();
        if (!ReferenceEquals(vgc, null))
        {
            Debug.Log("Adding a new video");
            VideoSurface vs = vgc.AddVideo(uid, isLocalUser);

            playerVideoList.Add(vs.gameObject);

            GameObject videoCanvasCamera = GameObject.Find("VideoCanvasCamera");
            videoCanvasCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Video Controller not found");
        }

        //// Update our "Content" container that holds all the newUserVideo image planes
        //content.sizeDelta = new Vector2(0, playerVideoList.Count * spaceBetweenUserVideos + 140);
    }

    private void RemoveUserVideoSurface(uint deletedUID)
    {
        foreach (GameObject player in playerVideoList)
        {
            if (player.name == deletedUID.ToString())
            {
                playerVideoList.Remove(player);
                Destroy(player.gameObject);
                break;
            }
        }
    }

    private void Cleaning()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            playerAnimator.SetBool("Cleaning", true);
        } else
        {
            playerAnimator.SetBool("Cleaning", false);
        }
        
    }

    private void CheckMovement()
    {
        // Checks using a made sphere with a radius if a certain layer is hit (in this case, Ground Layer)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Resets the speed of our fall
        if (isGrounded && velocity.y < 0)
        {
            // Not zero so that we don't immediately reset speed upon colliding with ground
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        playerAnimator.SetFloat("InputX", z);
        playerAnimator.SetFloat("InputY", x);

        move = transform.right * x + transform.forward * z;

        //if (Input.GetKey(KeyCode.LeftShift)) isRunning = true;
        //else isRunning = false;

        //if (isRunning) speed = 18f;
        //else speed = 12f;

        controller.Move(move * speed * Time.deltaTime);

        //if (Input.GetButtonDown("Jump") && isGrounded)
        //{
        //    // Formula for jumping
        //    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        //}

        velocity.y = gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if(transform.position.y > 2.0f)
        {
            transform.position = new Vector3(transform.position.x, 1.08f, transform.position.z);
        }
    }

    #region Agora Methods

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit, clean up...");
        TerminateAgoraEngine();
    }

    private void TerminateAgoraEngine()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine = null;
            IRtcEngine.Destroy();
        }
    }

    private void CheckAppId()
    {
        Debug.Assert(AppID.Length > 10, "AppId is problematic");
        if (AppID.Length > 10)
        {
            SetAppIdText();
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

        Join(channelName);
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
        //mRtcEngine.OnUserOffline = OnUserOffline;
        //mRtcEngine.OnVideoSizeChanged = OnVideoSizeChanged;
        // Calling virtual setup function

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
        //if (!ReferenceEquals(app, null))
        //{
        //    app = null; // delete app
        //    LeaveShareScreen();
        //    //SceneManager.LoadScene(SceneEnum.MainMenuScene.ToString(), LoadSceneMode.Single); 
        //}
    }

    public void LeaveShareScreen()
    {
        // TODO : Do something when player is out of share screen mode
    }

    public void GetAgoraToken(string channelName)
    {
        StartCoroutine(GetToken(channelName));
    }

    IEnumerator GetToken(string channelName)
    {
        Debug.Log("Channel Name : " + channelName);
        channelName = channelName.Substring(0, channelName.Length - 1);
        uint uid;
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
    private void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {

        if (!photonView.isMine)
        {
            return;
        }

        myUid = uid;

        Debug.Log("ONJOINCHANNELSUCCESS");

        CreateUserVideoSurface(uid, true);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    protected void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        //if (!PhotonView.isMine)
        //{
        //    Debug.Log("What is mine");
        //    return;
        //}
        //else
        //{
        //    Debug.Log("This is mine!");
        ////}

        //if (!ReferenceEquals(vgc, null))
        //{
        //    Debug.Log("Adding a new video");
        //    vgc.AddVideo(uid, false);
        //}
        UIDDictionary[uid] = false;

        CreateUserVideoSurface(uid, false);
    }

    private void OnUserJoinedHandler(uint uid, int elapsed)
    {
        if (!PhotonView.isMine)
        {
            return;
        }

        CreateUserVideoSurface(uid, false);
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
    private void OnLeaveChannelHandler(RtcStats stats)
    {
        if (!photonView.isMine)
        {
            return;
        }

        foreach (GameObject player in playerVideoList)
        {
            Destroy(player.gameObject);
        }
        playerVideoList.Clear();
    }

    // Remote User Leaves the Channel.
    private void OnUserOfflineHandler(uint uid, USER_OFFLINE_REASON reason)
    {
        if (!photonView.isMine)
        {
            return;
        }


        RemoveUserVideoSurface(uid);
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

}
