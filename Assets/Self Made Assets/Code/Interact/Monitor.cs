using agora_gaming_rtc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : Interactable
{
    [SerializeField]
    private GameObject screen;
    [SerializeField]
    private AgoraShareScreen shareScreenScript;

    private bool isOn = false;

    public bool isMainScreen = false;

    GameObject player = null;

    public override string GetDescription(GameObject player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator.GetBool("Sitting") == false)
        {
            return "";
        }
        return "Press E to use PC";
    }

    public override void Interact(GameObject player)
    {
        transform.GetComponent<PhotonView>().RequestOwnership();
        this.player = player;

        Debug.Log("Current player uid " + player.GetComponent<Player>().currUid);

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator.GetBool("Sitting"))
        {
            MouseLook ml = player.GetComponentInChildren<MouseLook>();
            GameObject ScreenUI = transform.Find("UI").gameObject;
            if (!isOn)
            {
                ScreenUI.SetActive(true);
                if (!ReferenceEquals(ml, null))
                {
                    if(!ReferenceEquals(shareScreenScript, null))
                    {
                        shareScreenScript.SetupUI(isMainScreen);
                        transform.GetComponent<PhotonView>().RPC("SetupProjector", PhotonTargets.AllBuffered, player.GetComponent<Player>().currUid.ToString());
                        ml.Lock();
                        ml.EnableCursor();
                    }
                    
                }
            } else
            {
                if (!ReferenceEquals(ml, null))
                {
                    VideoSurface vsScreen = screen.GetComponent<VideoSurface>();
                    //if (!ReferenceEquals(vsScreen, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsScreen);

                    if (isMainScreen)
                    {
                        VideoSurface vsWhiteboard = GameObject.Find("ProjectorScreen").GetComponent<VideoSurface>();
                        //if (!ReferenceEquals(vsWhiteboard, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsWhiteboard);
                    }

                    GetComponentInChildren<AgoraShareScreen>().StopScreenCapture(isMainScreen);
                    ml.Unlock();
                    ml.DisableCursor();
                    player = null;
                }
                ScreenUI.SetActive(false);
            }
            isOn = !isOn;
        }
    }

    public void Interact()
    {
        if (ReferenceEquals(player, null))
            return;

        transform.GetComponent<PhotonView>().RequestOwnership();
        Debug.Log("Current player uid " + player.GetComponent<Player>().currUid);

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator.GetBool("Sitting"))
        {
            MouseLook ml = player.GetComponentInChildren<MouseLook>();
            GameObject ScreenUI = transform.Find("UI").gameObject;
            if (!isOn)
            {
                ScreenUI.SetActive(true);
                if (!ReferenceEquals(ml, null))
                {
                    if (!ReferenceEquals(shareScreenScript, null))
                    {
                        shareScreenScript.SetupUI(isMainScreen);
                        transform.GetComponent<PhotonView>().RPC("SetupProjector", PhotonTargets.AllBuffered, player.GetComponent<Player>().currUid.ToString());
                        ml.Lock();
                        ml.EnableCursor();
                    }

                }
            }
            else
            {
                if (!ReferenceEquals(ml, null))
                {
                    VideoSurface vsScreen = screen.GetComponent<VideoSurface>();
                    //if (!ReferenceEquals(vsScreen, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsScreen);

                    if (isMainScreen)
                    {
                        VideoSurface vsWhiteboard = GameObject.Find("ProjectorScreen").GetComponent<VideoSurface>();
                        //if (!ReferenceEquals(vsWhiteboard, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsWhiteboard);
                    }

                    GetComponentInChildren<AgoraShareScreen>().StopScreenCapture(isMainScreen);
                    ml.Unlock();
                    ml.DisableCursor();
                    player = null;
                }
                ScreenUI.SetActive(false);
            }
            isOn = !isOn;
        }
    }

    [PunRPC]
    public void SetupProjector(string uidString)
    {
        uint uid = Convert.ToUInt32(uidString);

        VideoSurface vsScreen = screen.GetComponent<VideoSurface>();
        if (!ReferenceEquals(vsScreen, null))
        {
            Debug.Log("Hey VS is not null");
            if (player == null || player.GetComponent<Player>().currUid != uid)
            {
                Debug.Log("The uid string " + uidString);
                Debug.Log("The passed uid " + uid);
                Debug.Log("Setting up screen for other users!");
                vsScreen.SetForUser(uid);
            }
        }
        else
        {
            Debug.Log("Hey VS is null");
            if (!ReferenceEquals(shareScreenScript, null))
            {
                Debug.Log("Hey the share screen script is here!");
                shareScreenScript.ProjectShareScreen(isMainScreen);
                if (player == null || player.GetComponent<Player>().currUid != uid)
                {
                    Debug.Log("The uid string " + uidString);
                    Debug.Log("The passed uid " + uid);
                    vsScreen = screen.GetComponent<VideoSurface>();
                    Debug.Log("Setting up screen for other users!");
                    vsScreen.SetForUser(uid);
                }
            }
        }

        if (isMainScreen)
        {
            VideoSurface vsWhiteboard = GameObject.Find("ProjectorScreen").GetComponent<VideoSurface>();
            if (!ReferenceEquals(vsWhiteboard, null))
            {
                if (player == null || player.GetComponent<Player>().currUid != uid)
                {
                    Debug.Log("Setting up whiteboard for other users!");
                    vsWhiteboard.SetForUser(uid);
                }
                //transform.GetComponent<PhotonView>().ObservedComponents.Add(vsWhiteboard);
            }
        }
    }

    [PunRPC]
    public void DisableProjector(string uid)
    {

    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.isWriting)
        //{
        //    if(player != null)
        //    {
        //        Player playerScript = GetComponent<Player>();
        //        stream.SendNext(playerScript.currUid);
        //    }
        //}
        //else
        //{
        //    //isOpen = (bool)stream.ReceiveNext();

        //    if (player != null) broadcastingUid = (uint)stream.ReceiveNext();
        //    else broadcastingUid = uint.MaxValue;
        //}
    }

    public void BroadcastShareDisplay()
    {
        transform.GetComponent<PhotonView>().RPC("ShareDisplay", PhotonTargets.AllBuffered);
    }

    public void BroadcastShareWindow()
    {
        transform.GetComponent<PhotonView>().RPC("ShareWindow", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    public void ShareDisplay()
    {
        shareScreenScript.ShareDisplayScreen(false);
    }

    [PunRPC]
    public void ShareWindow()
    {
        shareScreenScript.OnShareWindowClick(false);
    }
}
