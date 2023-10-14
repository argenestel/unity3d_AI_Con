using System;
using System.Collections;
using System.Collections.Generic;
using uLipSync;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class PlayHT_TTS : MonoBehaviour
{
    public  AudioSource audioSource;
    public string url = "http://localhost:3000/tts";

    void OnEnable()
    {
        SpeechRecognitionTest.OnConversation += OnConversation;
    }

    private void OnConversation(string Input)
    {
        string newUri = url + "?input=" + Input;
        print(newUri);
        StartCoroutine(makePlayHTRequest(newUri));
    }

    IEnumerator makePlayHTRequest(string URI){
        UnityWebRequest webRequest = UnityWebRequest.Get(URI);

        yield return webRequest.SendWebRequest();

        if(webRequest.result == UnityWebRequest.Result.Success){
            string response = webRequest.downloadHandler.text;
            ResponseHandler(response);
        }
        else{
            Debug.LogWarning("Error: " + webRequest.error);
        }
    }

    private void ResponseHandler(string response)
    {
        try{
            print(response);
            var jsonObject = JsonConvert.DeserializeObject<JObject>(response);

        // Access the URL from the mp3Urls array
        var mp3Urls = jsonObject["mp3Urls"].ToObject<List<string>>();

        if (mp3Urls.Count > 0)
        {

             string DownloaderUrl = mp3Urls[0];
            print(DownloaderUrl);
            StartCoroutine(downloadMP3(DownloaderUrl));
            
        }
        else
        {
           Debug.LogWarning("cant get mp3urls");
        }

         
        }catch (Exception e){
            Debug.LogError(e);
        }
    }


    IEnumerator downloadMP3(string URI){
        UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(URI, AudioType.MPEG);

        yield return webRequest.SendWebRequest();

        if(webRequest.result == UnityWebRequest.Result.Success){
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else{
            Debug.LogWarning("Error: " + webRequest.error);
        }
    }

}
