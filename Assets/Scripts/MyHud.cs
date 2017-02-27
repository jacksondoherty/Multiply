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
	public Button lanHostButton;
	public Button lanClientButton;
	public InputField hostAddress;

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
		lanHostButton.onClick.AddListener (LANHost);
		lanClientButton.onClick.AddListener (LANClient);
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
				if (gameStarted && !gameScript.gameOver && 
					(Input.GetKeyDown (KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))) {
					gamePaused = !gamePaused;
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Q)) {
			Application.Quit();
		}
	}

	void OnGUI() {
		if (NetworkClient.active && !networkActive) {
			networkActive = true;
			lobbyMenu.SetActive (false);
			CancelInvoke ();
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
				// Internet Join match default
				// @TODO: put this in a more intuitive place
				pauseText.text = "Preparing match...";
				pauseText.enabled = true;
				Cursor.lockState = CursorLockMode.None;

				manager.StartMatchMaker ();
				matchNameInput.text = manager.matchName;
				// invoked is cancelled during match
				InvokeRepeating ("ReloadMatches", 0.0f, 2.0f);
				hostAddress.text = "localhost";
			}

			manager.matchName = matchNameInput.text;
			manager.networkAddress = hostAddress.text;
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

	void LANHost() {
		isHost = true;
		pauseText.text = "Waiting on Client...";
		manager.StartHost ();
	}

	void LANClient() {
		pauseText.text = "Waiting on Host...";
		manager.StartClient ();
	}
		
	void CreateMatch() {
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
		// never entered game -> no host
		if (localPlayerScript == null) {
			LeaveMatch ();
		} else {
			// Must run code on server to synce vars ->
			// 	only player objects can do remote procedure calls
			localPlayerScript.CmdEndGame();
		}
	}

	public void LeaveMatch() {
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

	// Internet Matches
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
