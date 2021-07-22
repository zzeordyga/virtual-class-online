using agora_gaming_rtc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : Interactable
{
    [SerializeField]
    private GameObject screen;
    private bool isOn = false;

    public bool isMainScreen = false;

    GameObject player = null;

    public uint broadcastingUid = uint.MaxValue;

    private void Start()
    {
        transform.GetComponent<PhotonView>().ObservedComponents.Add(GameObject.Find("ProjectorScreen").GetComponent<Transform>());
        transform.GetComponent<PhotonView>().ObservedComponents.Add(GetComponentInChildren<AgoraShareScreen>());
    }

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

        broadcastingUid = player.GetComponent<Player>().currUid;
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
                    AgoraShareScreen ss = GetComponentInChildren<AgoraShareScreen>();
                    if(!ReferenceEquals(ss, null))
                    {
                        ss.SetupUI(isMainScreen);

                        VideoSurface vsScreen = screen.GetComponent<VideoSurface>();
                        if (!ReferenceEquals(vsScreen, null)) transform.GetComponent<PhotonView>().ObservedComponents.Add(vsScreen);

                        if (isMainScreen)
                        {
                            VideoSurface vsWhiteboard = GameObject.Find("ProjectorScreen").GetComponent<VideoSurface>();
                            if(!ReferenceEquals(vsWhiteboard, null)) transform.GetComponent<PhotonView>().ObservedComponents.Add(vsWhiteboard);
                        }
                        
                        
                        ml.Lock();
                        ml.EnableCursor();
                    }
                    
                }
            } else
            {
                if (!ReferenceEquals(ml, null))
                {
                    VideoSurface vsScreen = screen.GetComponent<VideoSurface>();
                    if (!ReferenceEquals(vsScreen, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsScreen);

                    if (isMainScreen)
                    {
                        VideoSurface vsWhiteboard = GameObject.Find("ProjectorScreen").GetComponent<VideoSurface>();
                        if (!ReferenceEquals(vsWhiteboard, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsWhiteboard);
                    }

                    GetComponentInChildren<AgoraShareScreen>().StopScreenCapture(isMainScreen);
                    ml.Unlock();
                    ml.DisableCursor();
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

        broadcastingUid = player.GetComponent<Player>().currUid;
        transform.GetComponent<PhotonView>().RequestOwnership();
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
                    AgoraShareScreen ss = GetComponentInChildren<AgoraShareScreen>();
                    if (!ReferenceEquals(ss, null))
                    {
                        ss.SetupUI(isMainScreen);

                        VideoSurface vsScreen = screen.GetComponent<VideoSurface>();
                        if (!ReferenceEquals(vsScreen, null)) transform.GetComponent<PhotonView>().ObservedComponents.Add(vsScreen);

                        if (isMainScreen)
                        {
                            VideoSurface vsWhiteboard = GameObject.Find("ProjectorScreen").GetComponent<VideoSurface>();
                            if (!ReferenceEquals(vsWhiteboard, null)) transform.GetComponent<PhotonView>().ObservedComponents.Add(vsWhiteboard);
                        }

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
                    if (!ReferenceEquals(vsScreen, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsScreen);

                    if (isMainScreen)
                    {
                        VideoSurface vsWhiteboard = GameObject.Find("ProjectorScreen").GetComponent<VideoSurface>();
                        if (!ReferenceEquals(vsWhiteboard, null)) transform.GetComponent<PhotonView>().ObservedComponents.Remove(vsWhiteboard);
                    }

                    GetComponentInChildren<AgoraShareScreen>().StopScreenCapture(isMainScreen);
                    ml.Unlock();
                    ml.DisableCursor();
                }
                ScreenUI.SetActive(false);
            }
            isOn = !isOn;
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            if(player != null)
            {
                Player playerScript = GetComponent<Player>();
                stream.SendNext(playerScript.currUid);
            }
        }
        else
        {
            //isOpen = (bool)stream.ReceiveNext();

            if (player != null) broadcastingUid = (uint)stream.ReceiveNext();
            else broadcastingUid = uint.MaxValue;
        }
    }
}
