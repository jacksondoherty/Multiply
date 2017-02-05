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
	private int numPlayers = 0;
	private const int maxPlayers = 2;

	public void EnterGame() {
		numPlayers++;
		// matchmaker shouldn't allow more players to join after here
		if (numPlayers == maxPlayers) {
			gameStarted = true;
		}
	}

	public void EndGame() {
		gameEnded = true;
	}
}
