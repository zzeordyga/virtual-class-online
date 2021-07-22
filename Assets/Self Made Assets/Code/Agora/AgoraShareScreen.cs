using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

using agora_gaming_rtc;
using AgoraNative;

/// <summary>
/// this is an example of using ScreenSharing APIs for Desktops
/// </summary>
public class AgoraShareScreen : MonoBehaviour
{
    public GameObject screen;

    Dropdown WindowOptionDropdown;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    readonly List<AgoraNativeBridge.RECT> WinDisplays = new List<AgoraNativeBridge.RECT>();
#else
    List<uint> MacDisplays;
#endif
    int CurrentDisplay = 0;
    private IRtcEngine mRtcEngine = null;
    Texture2D mTexture;
    Rect mRect;

    private void Start()
    {
        SetupUI(false);
        mRtcEngine = Player.RtcEngine;
        // Creates a rectangular region of the screen.
        mRect = new Rect(0, 0, Screen.width, Screen.height);
        // Creates a texture of the rectangle you create.
        mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGBA32, false);
    }

    private void Update()
    {
        if(ReferenceEquals(mRtcEngine, null))
        {
            mRtcEngine = Player.RtcEngine;
        }
    }

    public void SetupUI(bool isMainScreen)
    {
        Dropdown dropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        if (dropdown != null)
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            MacDisplays = AgoraNativeBridge.GetMacDisplayIds();
            WindowList list = AgoraNativeBridge.GetMacWindowList();
            if (list != null)
            {
                dropdown.options = list.windows.Select(w =>
                    new Dropdown.OptionData(w.kCGWindowOwnerName + " | " + w.kCGWindowNumber)).ToList();
            }
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            // Monitor Display info
            var winDispInfoList = AgoraNativeBridge.GetWinDisplayInfo();
            if (winDispInfoList != null)
            {
                foreach (var dpInfo in winDispInfoList)
                {
                    WinDisplays.Add(dpInfo.MonitorInfo.monitor);
                }
            }

            // Window ID info
            Dictionary<string, System.IntPtr> winWinIdList;
            AgoraNativeBridge.GetDesktopWindowHandlesAndTitles(out winWinIdList);
            if (winWinIdList != null)
            {

                dropdown.options = (winWinIdList.Select(w =>
                    new Dropdown.OptionData(string.Format("{0, -20} | {1}",
                        w.Key.Substring(0, System.Math.Min(w.Key.Length, 20)), w.Value))).ToList());


            }
#endif
            WindowOptionDropdown = dropdown;
        }

        Button button = GameObject.Find("ShareWindowButton").GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnShareWindowClick(true));
        }

        button = GameObject.Find("ShareDisplayButton").GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ShareDisplayScreen(true));
        }

        button = GameObject.Find("StopShareButton").GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => { StopScreenCapture(true); });
        }

        button = GameObject.Find("LeaveButton").GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                transform.parent.gameObject.GetComponent<Monitor>().Interact();
                StopScreenCapture(true);
            });
        }

        VideoSurface vs;

        GameObject quad = GameObject.Find("DisplayPlane");
        if (ReferenceEquals(quad, null))
        {
            Debug.Log("Display ga null");
            Debug.Log("Error: failed to find DisplayPlane");
            return;
        }
        else
        {
            vs = quad.AddComponent<VideoSurface>();
            vs.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
        }

        ProjectShareScreen(isMainScreen);

    }

    public void ProjectShareScreen(bool isMainScreen)
    {
        VideoSurface vs;

        if (ReferenceEquals(screen, null))
        {
            Debug.Log("Display ga null");
            Debug.Log("Error: failed to find DisplayPlane");
            return;
        }
        else
        {
            vs = screen.AddComponent<VideoSurface>();
            vs.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);

            Monitor currMonitor = transform.GetComponentInParent<Monitor>();
        }

        if (isMainScreen)
        {
            GameObject whiteboard = GameObject.Find("ProjectorScreen");
            if (ReferenceEquals(whiteboard, null))
            {
                Debug.Log("Display ga null");
                Debug.Log("Error: failed to find DisplayPlane");
                return;
            }
            else
            {
                vs = whiteboard.AddComponent<VideoSurface>();
                vs.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);

                Monitor currMonitor = transform.GetComponentInParent<Monitor>();

                vs.EnableFilpTextureApply(true, false);
                vs.transform.Rotate(Vector3.up, 180);
            }
        }
    }

    public void RemoveProjection(bool isMainScreen)
    {
        VideoSurface vs;

        if (ReferenceEquals(screen, null))
        {
            Debug.Log("Display ga null");
            Debug.Log("Error: failed to find DisplayPlane");
            return;
        }
        else
        {
            Destroy(screen.GetComponent<VideoSurface>());
        }

        if (isMainScreen)
        {
            GameObject whiteboard = GameObject.Find("ProjectorScreen");
            if (ReferenceEquals(whiteboard, null))
            {
                Debug.Log("Display ga null");
                Debug.Log("Error: failed to find DisplayPlane");
                return;
            }
            else
            {
                vs = whiteboard.AddComponent<VideoSurface>();
                if (!ReferenceEquals(vs, null)) Destroy(screen.GetComponent<VideoSurface>());
            }
        }
    }

    public void StopScreenCapture(bool isMainScreen)
    {
        mRtcEngine.StopScreenCapture();
        RemoveProjection(isMainScreen);
    }

    public void ShareDisplayScreen(bool ignore)
    {
        if (ignore)
        {
            Monitor monitor = transform.parent.gameObject.GetComponent<Monitor>();
            if (!ReferenceEquals(monitor, null))
            {
                monitor.BroadcastShareDisplay();
            }
        }
        ScreenCaptureParameters sparams = new ScreenCaptureParameters
        {
            captureMouseCursor = true,
            frameRate = 15
        };

        if(mRtcEngine != null) mRtcEngine.StopScreenCapture();

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        mRtcEngine.StartScreenCaptureByDisplayId(MacDisplays[CurrentDisplay], default(Rectangle), sparams); 
        CurrentDisplay = (CurrentDisplay + 1) % MacDisplays.Count;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        ShareWinDisplayScreen(CurrentDisplay);
        CurrentDisplay = (CurrentDisplay + 1) % WinDisplays.Count;
#endif
    }

    void ShareWinDisplayScreen(int index)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        var screenRect = new Rectangle
        {
            x = WinDisplays[index].left,
            y = WinDisplays[index].top,
            width = WinDisplays[index].right - WinDisplays[index].left,
            height = WinDisplays[index].bottom - WinDisplays[index].top
        };
        Debug.Log(string.Format(">>>>> Start sharing display {0}: {1} {2} {3} {4}", index, screenRect.x,
            screenRect.y, screenRect.width, screenRect.height));
        var ret = mRtcEngine.StartScreenCaptureByScreenRect(screenRect,
            new Rectangle { x = 0, y = 0, width = 0, height = 0 }, default(ScreenCaptureParameters));
#endif
    }

    void TestRectCrop(int order)
    {
        // Assuming you have two display monitors, each of 1920x1080, position left to right:
        Rectangle screenRect = new Rectangle() { x = 0, y = 0, width = 1920 * 2, height = 1080 };
        Rectangle regionRect = new Rectangle() { x = order * 1920, y = 0, width = 1920, height = 1080 };

        int rc = mRtcEngine.StartScreenCaptureByScreenRect(screenRect,
            regionRect,
            default(ScreenCaptureParameters)
            );
        if (rc != 0) Debug.LogWarning("rc = " + rc);
    }

    public void OnShareWindowClick(bool ignore)
    {
        if (ignore)
        {
            Monitor monitor = transform.parent.gameObject.GetComponent<Monitor>();
            if (!ReferenceEquals(monitor, null))
            {
                monitor.BroadcastShareWindow();
            }
        }

        char[] delimiterChars = { '|' };
        if (WindowOptionDropdown == null) return;
        string option = WindowOptionDropdown.options[WindowOptionDropdown.value].text;
        if (string.IsNullOrEmpty(option))
        {
            return;
        }

        string wid = option.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries)[1];
        Debug.LogWarning(wid + " is chosen");
        mRtcEngine.StopScreenCapture();

        mRtcEngine.StartScreenCaptureByWindowId(int.Parse(wid), default(Rectangle), default(ScreenCaptureParameters));
    }
}