using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkManager))]

public class MyHud : MonoBehaviour {
	public Canvas hud;
	public Button createMatchButton;
	public InputField matchNameInput;
	public Button joinMatchButton;
	public Image grid;

	private NetworkManager manager;

	void Awake()
	{
		DontDestroyOnLoad(hud);
		// temp singleton pattern workaround
		if (FindObjectsOfType(hud.GetType()).Length > 1)
		{
			Destroy(hud.gameObject);
		}
			
		manager = GetComponent<NetworkManager>();
		Button btnScript = createMatchButton.GetComponent<Button> ();
		btnScript.onClick.AddListener (CreateInternetMatch);
	}


	void OnGUI() {
		if (NetworkClient.active) {
			hud.enabled = false;
		} else {
			hud.enabled = true;

			if (manager.matchMaker == null) {
				manager.StartMatchMaker ();
				matchNameInput.text = manager.matchName;
			}

			manager.matchName = matchNameInput.text;

			manager.matchMaker.ListMatches (0, 
											20, 
											"", 
											true, 
											0, 
											0,
											manager.OnMatchList);
			
			if (manager.matches != null) {
				
				// remove completed matches
				foreach (Transform child in grid.transform) {
					MyButton script = child.gameObject.GetComponent<MyButton> ();
					bool found = false;
					foreach (MatchInfoSnapshot match in manager.matches) {
						if (script.networkId == (ulong)match.networkId) {
							found = true;
						}
					}
					if (!found) {
						Destroy (child.gameObject);
					}
				}

				// add new matches
				foreach (MatchInfoSnapshot match in manager.matches) {
					bool found = false;
					foreach (Transform child in grid.transform) {
						MyButton script = child.gameObject.GetComponent<MyButton> ();
						if (script.networkId == (ulong)match.networkId) {
							found = true;
						}
					}
					if (!found) {
						Button btn = Instantiate (joinMatchButton, grid.transform);
						btn.GetComponentInChildren<Text> ().text = "Join Room: " + match.name;
						btn.GetComponent<MyButton> ().networkId = (ulong)match.networkId;
					}
				}
			}
		}
	}


	void CreateInternetMatch() {
		manager.matchMaker.CreateMatch (manager.matchName, 
										manager.matchSize, 
										true, 
										"", 
										"", 
										"", 
										0, 
										0, 
										manager.OnMatchCreate);
	}
}
