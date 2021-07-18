using agora_gaming_rtc;
using agora_utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoGroupController : MonoBehaviour
{
    private const float Offset = 100;

    [SerializeField]
    private GameObject video;
    private List<VideoController> _videoControllers = new List<VideoController>();
    private List<VideoController> VideoControllers
    {
        get { return _videoControllers; }
    }

    protected Dictionary<uint, VideoSurface> UserVideoDict = new Dictionary<uint, VideoSurface>();
    private VideoRawDataManager videoRawDataManager;
    protected const string SelfVideoName = "MyView";

    public void AddVideo()
    {
        string uid = PlayerNetwork.instance.PlayerInfo.UserId;
        uint hashedUID = GameManager.GetInt64HashCode(uid);

        GameObject videoWindow = Instantiate(video);
        videoWindow.transform.SetParent(transform, false);

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(hashedUID.ToString(), videoWindow);
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(hashedUID);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(30);
            videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);
            UserVideoDict[hashedUID] = videoSurface;
        }
    }


    protected VideoSurface makeImageSurface(string goName, GameObject videoWindow)
    {
        GameObject go = videoWindow.transform.Find("Screen").gameObject;

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
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
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
        go.AddComponent<RawImage>();
        // make the object draggable
        go.AddComponent<UIElementDragger>();
        GameObject canvas = GameObject.Find("VideoCanvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
            Debug.Log("add video view");
        }
        else
        {
            Debug.Log("Canvas is null video view");
        }
        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
        float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
        Debug.Log("position x " + xPos + " y: " + yPos);
        go.transform.localPosition = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(3f, 4f, 1f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

}
