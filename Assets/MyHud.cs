using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]

public class MyHud : MonoBehaviour {
	public NetworkManager manager;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
	}

	void OnGUI() {
		
	}
}
