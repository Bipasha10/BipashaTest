using UnityEngine;
using UnityEditor;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;
using SimpleJSON;
using UnityEngine.UI;

public class APIHandler : MonoBehaviour {

	private readonly string basePath = "https://test.iamdave.ai/conversation/exhibit_aldo/74710c52-42a5-3e65-b1f0-2dc39ebe42c2";
	private RequestHelper currentRequest;
	[SerializeField]
	private Text responseText;

	private void LogMessage(string title, string message) {
#if UNITY_EDITOR
		EditorUtility.DisplayDialog (title, message, "Ok");
#else
		Debug.Log(message);
#endif
	}
	
	public void GetMensData(int index){
		string customerState = null;
		switch(index)
		{
			case 0:
				customerState = "cs_men_products";
                break;
			case 1:
				customerState = "cs_men_explore";
                break;
			case 2:
				customerState = "cs_about_collection";
                break;
		}
		currentRequest = new RequestHelper
		{
			Uri = basePath,
			Params = new Dictionary<string, string> {
				{ "system_response", "sr_init" },
				{ "engagement_id", "NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyZXhoaWJpdF9hbGRv" },
				{ "customer_state",customerState }
			},
			
			Headers = new Dictionary<string, string> {
					{ "X-I2CE-ENTERPRISE-ID", "dave_expo" },
					{"X-I2CE-USER-ID", "74710c52-42a5-3e65-b1f0-2dc39ebe42c2" },
					{"X-I2CE-API-KEY","NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyMTYwNzIyMDY2NiAzNw__" }
				},
			EnableDebug = true
		};
		RestClient.Post(currentRequest)
		.Then(response => {
		
			// And later we can clear the default query string params for all requests
			RestClient.ClearDefaultParams();			
			JSONNode nde = JSONNode.Parse(response.Text);
			Debug.Log(nde["response_channels"].Value);
			Debug.Log(nde["response_channels"]["voice"].Value);
			responseText.text = response.Text;
			DownloadFile(nde["response_channels"]["voice"].Value);
			
        })
		.Catch(err => Debug.LogError("Error"+ err.Message));
	}

	

	public void DownloadFile(string audioUrl){

		var fileUrl = audioUrl;
		var fileType = AudioType.WAV;

		RestClient.Get(new RequestHelper {
			Uri = fileUrl,
			DownloadHandler = new DownloadHandlerAudioClip(fileUrl, fileType)
		}).Then(res => {
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = ((DownloadHandlerAudioClip)res.Request.downloadHandler).audioClip;
			audio.Play();
		}).Catch(err => {
			this.LogMessage("Error", err.Message);
		});
	}
}