using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject loginMenuCanvas;
    public GameObject mainMenuCanvas;
    public GameObject lobbyNetwork;

    private void Start()
    {
        SwitchMenu(1);
    }

    public void SwitchMenu(int flag)
    {
        if (flag == 1)
        {
            menuCanvas.SetActive(true);
            loginMenuCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);
        }
        else if (flag == 2)
        {
            menuCanvas.SetActive(false);
            loginMenuCanvas.SetActive(true);
            mainMenuCanvas.SetActive(false);
        }
        else if (flag == 3)
        {
            menuCanvas.SetActive(false);
            loginMenuCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
