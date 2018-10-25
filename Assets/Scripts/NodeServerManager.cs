﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NodeServerManager : MonoBehaviour {

	private static string url = "http://localhost";
	private static string port = "3000";
	int running;
	EmotionColorPicker colorPicker;
	public static string confirmedFullText;
	public static string currentPossibleText;

	public delegate void APICallReturnedDelegate();
	public static APICallReturnedDelegate APIReturned;
	
	// Use this for initialization
	void Awake () {
		running = 0;
		confirmedFullText = "";
		currentPossibleText = "";
		//StartNodeServer();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) {
			ChangeRunningStatus(running == 0 ? 1 : 0);
		}	
	}

	public void ChangeRunningStatus(int newStatus) {
		
		if(newStatus == 1) {
			if(running == 0) {
				StartCoroutine(ChangeServerStatus("start"));
				running = 1;
				StartCoroutine(SendRequests());
			}
		}
		else {
			if(running == 1) {
				StartCoroutine(ChangeServerStatus("stop"));
				running = 0;
			}
		}
	}

	void StartNodeServer(){
		System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
		string path = Application.dataPath.Substring(0,Application.dataPath.Length - 7);
		Debug.Log(Application.dataPath.Substring(0,Application.dataPath.Length - 7));
		proc.FileName = "/bin/sh";
		proc.UseShellExecute = true;
		proc.CreateNoWindow = false;
		proc.Arguments = path + "node server.js";
		System.Diagnostics.Process.Start(proc);
	}

	IEnumerator ChangeServerStatus(string action) {
		WWWForm form = new WWWForm();
		form.AddField("action", action);
		UnityWebRequest www = UnityWebRequest.Post(url + ":" + port, form);
		yield return www.Send();

		if(www.isNetworkError) {
			Debug.Log(www.error);
		}
		else {
			Debug.Log("Server " + action);
		}
	}

	IEnumerator SendRequests(){
		while(running == 1) {
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
					confirmedFullText += www.downloadHandler.text;
					APIReturned();
				}
				else if(www.responseCode == 206) {
					Debug.Log("Received Guess");
					currentPossibleText = www.downloadHandler.text;
					APIReturned();
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

	void OnApplicationQuit() {
		StartCoroutine(ChangeServerStatus("stop"));
	}
}