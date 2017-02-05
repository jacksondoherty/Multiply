using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class MyButton : MonoBehaviour {
	public NetworkManager manager;
	public MatchInfoSnapshot match;

	void Start() {
		GetComponent<Button> ().onClick.AddListener (JoinMatch);
	}

	void JoinMatch() {
		if (manager.matchMaker != null) {
			manager.matchMaker.JoinMatch (
				match.networkId, 
				"", 
				"", 
				"", 
				0, 
				0, 
				manager.OnMatchJoined);
		}
	}
}
