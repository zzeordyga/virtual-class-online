using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{
    public async void Login()
    {
        SceneManager.LoadScene("LoginAPIScene");
    }


}
