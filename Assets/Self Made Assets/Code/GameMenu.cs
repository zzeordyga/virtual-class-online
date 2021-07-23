using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    private bool isActive = false;

   public void LeaveRoom()
    {
        transform.parent.GetComponent<Player>().TerminateAgoraEngine();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenuScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isActive = !isActive;
            transform.Find("Menu").gameObject.SetActive(isActive);
            MouseLook ml = transform.parent.GetComponentInChildren<MouseLook>();
            if (isActive)
            {
                EnableCursor();
                ml.Lock();
            } else
            {
                DisableCursor();
                ml.Unlock();
            }
        }
    }

    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
