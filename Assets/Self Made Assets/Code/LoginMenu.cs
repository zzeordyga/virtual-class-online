using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class LoginMenu : MonoBehaviour
{

    private string salt;
    private string player;
    private static readonly HttpClient client = new HttpClient();

    public async void Login()
    {
        await GetSaltInfo();
        SaltInfo saltInfo = JsonUtility.FromJson<SaltInfo>(salt);
        salt = saltInfo.Salt;
        AES.encrypt(salt);

        //await GetPlayerInfo();
        //PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(player);
    }

    public async Task GetSaltInfo()
    {
        string url = "https://laboratory.binus.ac.id/lapi/api/Account/Salt/" + "username";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.ConnectionError)
                Debug.Log(request.error);
            else
                salt =  request.downloadHandler.text;
        }
    }

    public async Task GetPlayerInfo()
    {

    }

}
