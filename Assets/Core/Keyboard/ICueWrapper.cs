using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class ICueWrapper : MonoBehaviour
{

    public void UpdateKeyboardState(bool isConsoleOpen)
    {
        if (isConsoleOpen)
        {
            StartCoroutine(SetConsoleOpen(true));
            StartCoroutine(SetConsoleClose(false));
        }
        else
        {
            StartCoroutine(SetConsoleOpen(false));
            StartCoroutine(SetConsoleClose(true));
        }
    }


    IEnumerator SetConsoleOpen(bool state)
    {

        byte[] data = new byte[] {};
        string link = "http://localhost:25555/api/profiles/ConsoleOpen/state/" + state.ToString();

        using (UnityWebRequest request = UnityWebRequest.Put(link, data))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSONNode.Parse(json);
                Debug.Log(stats);
            }
        }
    }

    IEnumerator SetConsoleClose(bool state)
    {

        byte[] data = new byte[] { };
        string link = "http://localhost:25555/api/profiles/ConsoleClose/state/" + state.ToString();

        using (UnityWebRequest request = UnityWebRequest.Put(link, data))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSONNode.Parse(json);
                Debug.Log(stats);
            }
        }
    }
}
