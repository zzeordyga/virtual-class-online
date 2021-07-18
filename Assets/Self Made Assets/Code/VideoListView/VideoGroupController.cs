using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoGroupController : MonoBehaviour
{
    [SerializeField]
    private GameObject video;
    private List<VideoController> _videoControllers = new List<VideoController>();
    private List<VideoController> VideoControllers
    {
        get { return _videoControllers; }
    }

    public void AddVideo(Player player)
    {
        GameObject videoWindow = Instantiate(video);
        videoWindow.transform.SetParent(transform, false);
    }
}
