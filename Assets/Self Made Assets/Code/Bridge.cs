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
    class ObjectSalt{
        public string Challenge;
        public string Salt; 
    }

    ObjectSalt objectSalt;

    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    
    private string saltUrl = "https://laboratory.binus.ac.id/lapi/api/Account/Salt/";
    private string loginUrl = "https://laboratory.binus.ac.id/lapi/api/Account/LogOnBPlus";

    void Start()
    {
        passwordField.contentType = TMP_InputField.ContentType.Password;
    }

    public void login() {
        StartCoroutine(getSalt());
    }

    IEnumerator getSalt() {
        string username = usernameField.text.ToUpper();
        UnityWebRequest uwr = UnityWebRequest.Get(saltUrl + username);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            objectSalt = JsonUtility.FromJson<ObjectSalt>(uwr.downloadHandler.text);
                
            string salt = objectSalt.Salt;
            Debug.Log("Salt: " + salt);
            StartCoroutine(getLogin(salt));
        }
    }

    IEnumerator getLogin(string salt) {
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
                Debug.Log(www.downloadHandler.text);
                if(www.downloadHandler.text != null){
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
    }

    public void back(){
        SceneManager.LoadScene("LoginScene");
    }
}
