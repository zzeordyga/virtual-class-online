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

    //add and remove video here
    public void AddVideo(Player player)
    {
        // Please add this to the list of videos (i have no idea how jose help)
        GameObject videoWindow = Instantiate(video);

        videoWindow.transform.SetParent(transform, false);
    }


}
