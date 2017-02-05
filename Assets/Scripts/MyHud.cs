using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkManager))]

public class MyHud : NetworkBehaviour {

	public Canvas hud;
	public GameObject lobbyMenu;
	public Button createMatchButton;
	public InputField matchNameInput;
	public Button joinBtnPrefab;
	public Image grid;
	public bool gamePaused = false;
	public GameObject gamePauseMenu;
	public Button endGameButton;

	private NetworkManager manager;
	private Game game;
	private bool isHost = false;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
		createMatchButton.onClick.AddListener (CreateMatch);
		endGameButton.onClick.AddListener (EndGame);
	}

	void Update() {
		if (NetworkClient.active) {
			if (manager.clientLoadedScene) {
				if (game == null) {
					GameObject gameObj = GameObject.Find("Game");
					game = gameObj.GetComponent<Game> ();
				}
				if (game.gameEnded) {
					LeaveMatch ();
				}
			}
			if (Input.GetKeyDown (KeyCode.P)) {
				gamePaused = !gamePaused;
			}
		}
	}

	void OnGUI() {
		if (NetworkClient.active) {
			lobbyMenu.SetActive (false);
			gamePauseMenu.SetActive (gamePaused);
		} else {
			lobbyMenu.SetActive (true);
			gamePauseMenu.SetActive (false);

			// matchMaker becomes null after exiting match
			// thus enters here after each completed match
			if (manager.matchMaker == null) {
				gamePaused = false;
				manager.StartMatchMaker ();
				matchNameInput.text = manager.matchName;
				// invoked is cancelled during match
				InvokeRepeating ("ReloadMatches", 0.0f, 2.0f);
			}

			manager.matchName = matchNameInput.text;
		}
	}
		
	void CreateMatch() {
		CancelInvoke ();
		isHost = true;
		manager.matchMaker.CreateMatch (
			manager.matchName, 
			manager.matchSize, 
			true, 
			"", 
			"", 
			"", 
			0, 
			0, 
			manager.OnMatchCreate);
	}

	void EndGame() {

		// Must run code on server to synce vars ->
		// 	only player objects can do remote procedure calls
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; i++) {
			PlayerController script = players [i].GetComponent<PlayerController> ();
			if (script.isLocalPlayer) {
				script.CmdEndGame ();
				break;
			}
		}
	}

	void LeaveMatch() {
		if (isHost) {
			manager.StopHost ();
			isHost = false;
		} else {
			manager.StopClient ();
		}
		manager.StopMatchMaker ();
	}

	void ReloadMatches() {
		manager.matchMaker.ListMatches (
			0, 
			20, 
			"", 
			true, 
			0, 
			0,
			manager.OnMatchList);
		UpdateButtons ();
	}

	void UpdateButtons() {
		if (manager.matches != null) {
			// remove completed matches
			foreach (Transform child in grid.transform) {
				MyButton script = child.gameObject.GetComponent<MyButton> ();
				bool found = false;
				foreach (MatchInfoSnapshot match in manager.matches) {
					if (script.match.networkId == match.networkId) {
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
					if (script.match.networkId == match.networkId) {
						found = true;
					}
				}
				if (!found) {
					Button btn = Instantiate (joinBtnPrefab, grid.transform);
					btn.GetComponentInChildren<Text> ().text = "Join Room: " + match.name;
					MyButton script = btn.GetComponent<MyButton> ();
					script.manager = manager;
					script.match = match;
				}
			}
		}
	}
}
