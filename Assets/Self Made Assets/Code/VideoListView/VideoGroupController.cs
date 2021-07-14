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
}
