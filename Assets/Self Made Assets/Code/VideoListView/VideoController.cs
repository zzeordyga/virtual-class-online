using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using agora_gaming_rtc;

public class VideoController : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;

    private uint _uID;
    public uint UID
    {
        get { return _uID; }
        set { _uID = value; }
    }

    private Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    private VideoRawDataManager videoRawDataManager;
    protected const string SelfVideoName = "MyView";

    public VideoSurface Init(uint uid, bool isLocalUser)
    {
        UID = uid;

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(UID.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            videoSurface.SetGameFps(30);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
            videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);

            if (!isLocalUser)
            {
                Debug.Log("Is not Local");
                videoSurface.SetForUser(UID);
            }
            else
            {
                Debug.Log("is local");
            }

        }

        return videoSurface;
    }

    protected VideoSurface makeImageSurface(string goName)
    {
        GameObject go = transform.Find("Screen").gameObject;

        if (ReferenceEquals(go, null))
        {
            return null;
        }

        go.name = goName;

        RawImage rawImage = go.GetComponent<RawImage>();
        rawImage.rectTransform.sizeDelta = new Vector2(1, 1);// make it almost invisible

        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);

        // configure videoSurface
        VideoSurface videoSurface = go.GetComponent<VideoSurface>();
        return videoSurface;
    }
}
