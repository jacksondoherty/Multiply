using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Game : NetworkBehaviour {

	[SyncVar]
	public bool gameEnded = false;
	[SyncVar]
	public bool gameStarted = false;
	[SyncVar]
	public bool gameOver = false;
	[SyncVar]
	public NetworkInstanceId winnerId;

	[SyncVar]
	private NetworkInstanceId playerOneId;
	[SyncVar]
	private NetworkInstanceId playerTwoId;
	[SyncVar]
	private int numPlayers = 0;
	private const int maxPlayers = 2;

	public void EnterGame() {
		numPlayers++;
		// matchmaker shouldn't allow more players to join after here
		if (numPlayers == maxPlayers) {
			gameStarted = true;
		}
	}

	public void EnterGame(NetworkInstanceId id) {
		numPlayers++;
		if (numPlayers == 1) {
			playerOneId = id;
		} else if (numPlayers == 2) {
			playerTwoId = id;
		} else {
			print ("Error: more than 2 players");
		}
		// matchmaker shouldn't allow more players to join after here
		if (numPlayers == maxPlayers) {
			gameStarted = true;
		}
	}

	public void EndGame() {
		gameEnded = true;
	}

	public void RegisterDeath(NetworkInstanceId id) { 
		gameOver = true;
		if (id == playerOneId) {
			winnerId = playerTwoId;
		} else {
			winnerId = playerOneId;
		}
	}

	void Update() {
		//DEBUG
		if (Input.GetKeyDown (KeyCode.I)) {
			PrintNetIDs ();
		}
	}

	void PrintNetIDs() {
		print ("player one: " + playerOneId);
		print ("player two: " + playerTwoId);
	}
}
