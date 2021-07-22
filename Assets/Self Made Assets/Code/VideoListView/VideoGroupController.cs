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

    public VideoSurface AddVideo(uint uid, bool isLocalUser)
    {
        if (UserVideoDict.ContainsKey(uid))
        {
            Debug.Log("User is logged in");
            return null;
        }

        GameObject videoWindow = Instantiate(video);
        if (videoWindow == null)
        {
            Debug.LogError("CreateUserVideoSurface() - newUserVideoIsNull");
            return null;
        }
        videoWindow.name = uid.ToString();
        videoWindow.transform.SetParent(this.transform, false);
        //videoWindow.transform.rotation = Quaternion.Euler(Vector3.right * -180);


        // Update our VideoSurface to reflect new users
        VideoSurface newVideoSurface = videoWindow.GetComponent<VideoSurface>();
        if (newVideoSurface == null)
        {
            Debug.LogError("CreateUserVideoSurface() - VideoSurface component is null on newly joined user");
            return null;
        }

        if (isLocalUser == false)
        {
            newVideoSurface.SetForUser(uid);
        }
        newVideoSurface.SetGameFps(30);

        return newVideoSurface;
    }

    public void RemoveVideo(uint UiD)
    {
        UserVideoDict.Remove(UiD);
        Destroy(GameObject.Find(UiD.ToString()));
    }

}
