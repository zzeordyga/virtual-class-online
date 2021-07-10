using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using UnityEngine.Networking;
using AES;
using TMPro;

public class Bridge : MonoBehaviour
{
    SaltInfo objectSalt;

    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    
    private string saltUrl = "https://laboratory.binus.ac.id/lapi/api/Account/Salt/";
    private string loginUrl = "https://laboratory.binus.ac.id/lapi/api/Account/LogOnBPlus";

    void Start()
    {
        passwordField.contentType = TMP_InputField.ContentType.Password;
    }

    private void OnEnable()
    {
        if (usernameField.text != "" && passwordField.text != "")
        {
            usernameField.text = "";
            passwordField.text = "";
        }
    }

    public void Login() {
        StartCoroutine(GetSalt());
    }

    IEnumerator GetSalt() {
        string username = usernameField.text.ToUpper();
        UnityWebRequest uwr = UnityWebRequest.Get(saltUrl + username);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            objectSalt = JsonUtility.FromJson<SaltInfo>(uwr.downloadHandler.text);
                
            string salt = objectSalt.Salt;
            Debug.Log("Salt: " + salt);
            StartCoroutine(GetLogin(salt));
        }
    }

    IEnumerator GetLogin(string salt) {
        string username = usernameField.text.ToUpper();
        string password = passwordField.text;
        string hashPassword = AES.General.EncryptToBase64(salt + username, password);
        Debug.Log("Hash: " + hashPassword);
        WWWForm form = new WWWForm();
        form.AddField("userName", username);
        form.AddField("password", hashPassword);
        form.AddField("isPersistent", "false");

        using (UnityWebRequest www = UnityWebRequest.Post(loginUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                if(www.downloadHandler.text != null){
                    PlayerNetwork.instance.PlayerInfo = (PlayerInfo)JsonUtility.FromJson<PlayerInfo>(www.downloadHandler.text);
                    LobbyNetwork.Init();
                } else
                {
                    usernameField.text = "";
                    passwordField.text = "";
                }
            }
        }
    }

    public void Back(){
        SceneManager.LoadScene("LoginScene");
    }
}
