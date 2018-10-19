﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetTextFromServer : MonoBehaviour {

	private static string url = "http://localhost";
	private static string port = "3000";
	Text displayText;
	public Text fullText;
	EmotionColorPicker colorPicker;
	
	// Use this for initialization
	void Start () {
		displayText = GetComponent<Text>();
		colorPicker = GetComponent<EmotionColorPicker>();

		StartCoroutine(SendRequests());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SendRequests(){
		while(true) {
			UnityWebRequest www = UnityWebRequest.Get(url +":" + port);
			yield return www.Send();

			if(www.isNetworkError)
			{
				Debug.Log(www.error);
			}
			else
			{
				if(www.responseCode == 200) {
					Debug.Log("Form sent complete!");
					Debug.Log("response:" + www.downloadHandler.text);
					fullText.text += www.downloadHandler.text;
					string[] words = fullText.text.Split(" ".ToCharArray());
					displayText.text = words[words.Length - 1];
					colorPicker.UpdateBackgroundColor(words[words.Length - 1]);
				}
				else if(www.responseCode == 206) {
					Debug.Log("Received Guess");
					string guess = www.downloadHandler.text;
					string[] words = guess.Split(" ".ToCharArray());
					displayText.text = words[words.Length - 1];
				}
				else if(www.responseCode ==204) {
					//Debug.Log("no new content!");
				}
				else {
					Debug.Log("Error response code: " + www.responseCode.ToString());
				}
			}
		}
	}
}