using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoController : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture webcam;
    private Texture defaultBackground;
    [SerializeField] private TMP_Text playerName;

    public RawImage background;
    public AspectRatioFitter fit;

    private Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    //private void Start()
    //{
    //    playerName.text = PhotonNetwork.player.CustomProperties["Name"].ToString();

    //    defaultBackground = background.texture;

    //    WebCamDevice[] devices = WebCamTexture.devices;

    //    Debug.Log("Masuk kamera gan");

    //    if (devices.Length == 0)
    //    {
    //        Debug.Log("No camera detected");
    //        camAvailable = false;
    //        return;
    //    }

    //    for (int i = 0; i < devices.Length; i++)
    //    {
    //        Debug.Log(devices[i].name);

    //        // Still using the whole screen
    //        webcam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
    //    }

    //    if (webcam != null)
    //    {
    //        webcam.Play();
    //        background.texture = webcam;

    //        camAvailable = true;
    //    }

    //}

    //private void Update()
    //{
    //    if (!camAvailable) return;

    //    if(true)
    //    {
    //        float ratio = (float)webcam.width / (float)webcam.height;
    //        fit.aspectRatio = ratio;

    //        float scaleY = webcam.videoVerticallyMirrored ? -1f : 1f;
    //        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

    //        int orient = -webcam.videoRotationAngle;
    //        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    //    }

    //}

}
