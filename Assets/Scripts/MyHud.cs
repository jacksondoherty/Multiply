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
	public Button endMatchButton;
	public Text pauseText;

	private NetworkManager manager;
	private Game gameScript;
	private bool isHost = false;
	private bool gameStarted = false;
	private PlayerController localPlayerScript;
	private bool networkActive = true;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
		createMatchButton.onClick.AddListener (CreateMatch);
		endMatchButton.onClick.AddListener (EndGame);
	}

	void Update() {
		if (NetworkClient.active) {
			if (localPlayerScript == null) {
				SetupLocalPlayerRef ();
			}
			if (gameScript == null) {
				GameObject gameObj = GameObject.Find ("Game");
				//@Todo - call method to identify when scene has loaded as to not do this
				if (gameObj != null) {
					gameScript = gameObj.GetComponent<Game> ();
				}
			} else {
				if (gameScript.gameEnded) {
					LeaveMatch ();
				} else if (gameScript.gameOver) {
					gamePaused = true;
					if (gameScript.winnerId == localPlayerScript.netId) {
						pauseText.text = "You won!";
					} else {
						pauseText.text = "You lost.";
					}
					pauseText.enabled = true;
				// when game starts (both players have entered)
				} else if (gameScript.gameStarted && !gameStarted) {
					gameStarted = true;
					pauseText.enabled = false;
					gamePaused = false;
				}
				if (gameStarted && !gameScript.gameOver && Input.GetKeyDown (KeyCode.P)) {
					gamePaused = !gamePaused;
				}
			}
		}
	}

	void OnGUI() {
		if (NetworkClient.active && !networkActive) {
			networkActive = true;
			lobbyMenu.SetActive (false);
		}  else if (!NetworkClient.active && networkActive) {
			networkActive = false;
			lobbyMenu.SetActive (true);
			gamePauseMenu.SetActive (false);
		}

		if (networkActive) {
			gamePauseMenu.SetActive (gamePaused);
		} else {
			// matchMaker becomes null after exiting match
			// thus enters here after each completed match
			if (manager.matchMaker == null) {
				// reset settings
				isHost = false;
				gameStarted = false;
				gamePaused = true;
				pauseText.text = "Preparing match...";
				pauseText.enabled = true;

				manager.StartMatchMaker ();
				matchNameInput.text = manager.matchName;
				// invoked is cancelled during match
				InvokeRepeating ("ReloadMatches", 0.0f, 2.0f);
			}

			manager.matchName = matchNameInput.text;
		}
	}

	void SetupLocalPlayerRef() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; i++) {
			PlayerController script = players [i].GetComponent<PlayerController> ();
			if (script.isLocalPlayer) {
				localPlayerScript = script;
			}
		}
	}
		
	void CreateMatch() {
		CancelInvoke ();
		isHost = true;
		pauseText.text = "Waiting for 2nd player to join...";
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
		localPlayerScript.CmdEndGame();
	}

	void LeaveMatch() {
		if (isHost) {
			manager.StopHost ();
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
